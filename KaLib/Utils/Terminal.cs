using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using KaLib.Texts;

namespace KaLib.Utils;

public static class Terminal
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
    private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();

    [DllImport("kernel32")]
    private static extern bool SetConsoleCP(uint cp);
        
    [DllImport("kernel32")]
    private static extern bool SetConsoleOutputCP(uint cp);
    
    public delegate List<string> AutoCompleterDelegate(string input, int cursor);

    public delegate void InputBufferRendererDelegate(string input, string suggestion, int suggestionCursor, int cursor);
    
    private static SemaphoreSlim _writeLock = new(1, 1);

    private static List<char> _inputBuffer = new();
    private static string _suggesting = "";
    private static int _suggestApplyFrom = 0;
    private static List<string> _inputHistory = new();
    private static int _historyIndex = -1;
    private static bool _browsingHistory;
    private static int _inputIndex = 0;
    private static Text _currentPrompt = null;
    private static SemaphoreSlim _readLineLock = new(1, 1);

    private static int _stdoutCursorX;
    private static int _stdoutCursorY;
    
    static Terminal()
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
        
        // Force the console to use UTF-8
        SetConsoleCP(65001);
        SetConsoleOutputCP(65001);
                
        // Write ASCII colored text on Windows
        var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleMode(iStdOut, out var outConsoleMode);
        outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
        SetConsoleMode(iStdOut, outConsoleMode);
    }

    public static void WriteStdOut(string text)
    {
        (Console.CursorLeft, Console.CursorTop) = (_stdoutCursorX, _stdoutCursorY);
        Write(text);
        (_stdoutCursorX, _stdoutCursorY) = (Console.CursorLeft, Console.CursorTop);
    }

    public static void WriteStdOut(Text text) => WriteStdOut(text.ToAscii());

    public static void WriteLineStdOut(string text)
    {
        (Console.CursorLeft, Console.CursorTop) = (_stdoutCursorX, _stdoutCursorY);
        Write(text);
        WriteLine("".PadRight(Console.BufferWidth - Console.CursorLeft - 1));
        var pc = Console.WindowTop + Console.WindowHeight - 1;
        (_stdoutCursorX, _stdoutCursorY) = (Console.CursorLeft, Console.CursorTop);
        if (pc == Console.CursorTop) WriteLine("");
    }

    public static void WriteLineStdOut(Text text) => WriteLineStdOut(text.ToAscii());
    
    public static void Write(string text)
    {
        _writeLock.Wait();
        Console.Write(text);
        _writeLock.Release();
    }

    public static void Write(Text text) => Write(text.ToAscii());
    
    public static void WriteLine(string text)
    {
        _writeLock.Wait();
        Console.WriteLine(text);
        _writeLock.Release();
    }

    public static string CurrentInput => string.Join("", _inputBuffer);

    public static void WriteLine(Text text) => WriteLine(text.ToAscii());

    public static void ClearLine()
    {
        Console.CursorLeft = 0;
        Write(" ".PadRight(Console.BufferWidth - 1));
        Console.CursorLeft = 0;
    }

    public static void ClearRemaining()
    {
        Write(" ".PadRight(Console.BufferWidth - 1 - Console.CursorLeft));
    }
    
    public static void DrawPromptLine(string text = null, InputBufferRendererDelegate inputBufferRenderer = null)
    {
        Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
        Console.CursorLeft = 0;
        if (_currentPrompt != null) Write(_currentPrompt);
        var c = Console.CursorLeft;

        while (_inputBuffer.Count + c + 1 > Console.BufferWidth)
        {
            _inputBuffer.RemoveAt(_inputBuffer.Count - 1);
            if (_inputIndex > _inputBuffer.Count) _inputIndex = _inputBuffer.Count;
        }

        var line = text ?? (_browsingHistory && _historyIndex >= 0 ? 
            _inputHistory[_inputHistory.Count - _historyIndex - 1] :
            CurrentInput);

        inputBufferRenderer ??= (str, _, _, _) =>
        {
            Write(str);
            Write(" ".PadRight(Console.BufferWidth - 1 - Console.CursorLeft));
        };
        inputBufferRenderer(line, _suggesting, _suggestApplyFrom, _inputIndex);
        
        var c2 = Console.CursorLeft;
        Console.CursorLeft = _browsingHistory ? c2 : c + _inputIndex;
    }

    public static string ReadLine(string prompt, AutoCompleterDelegate autoCompleter = null, InputBufferRendererDelegate inputBufferRenderer = null) =>
        ReadLine(LiteralText.Of(prompt), autoCompleter, inputBufferRenderer);

    public static string ReadLine(Text prompt = null, AutoCompleterDelegate autoCompleter = null, InputBufferRendererDelegate inputBufferRenderer = null)
    {
        _readLineLock.Wait();
        _currentPrompt = prompt;
        DrawPromptLine();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Modifiers.HasFlag(ConsoleModifiers.Control)) continue;
            if (key.Modifiers.HasFlag(ConsoleModifiers.Alt)) continue;

            if (key.Key is ConsoleKey.UpArrow or ConsoleKey.DownArrow)
            {
                if (_inputHistory.Count > 0)
                {
                    _browsingHistory = true;
                    _historyIndex += key.Key == ConsoleKey.UpArrow ? 1 : -1;
                    _historyIndex = Math.Max(Math.Min(_inputHistory.Count - 1, _historyIndex), -1);
                }

                DrawPromptLine(inputBufferRenderer: inputBufferRenderer);
                continue;
            }

            if (_browsingHistory)
            {
                _browsingHistory = false;
                _inputBuffer.Clear();
                _inputBuffer.AddRange(_inputHistory[_inputHistory.Count - 1 - _historyIndex]);
                _inputIndex = _inputBuffer.Count;
                _historyIndex = -1;
            }

            var handleNeeded = false;
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    _inputIndex = Math.Max(_inputIndex - 1, 0);
                    break;
                case ConsoleKey.Backspace:
                    if (_inputIndex > 0) _inputBuffer.RemoveAt(_inputIndex - 1);
                    _inputIndex = Math.Max(_inputIndex - 1, 0);
                    break;
                case ConsoleKey.Delete:
                    if (_inputIndex < _inputBuffer.Count) _inputBuffer.RemoveAt(_inputIndex);
                    break;
                case ConsoleKey.RightArrow:
                    _inputIndex = Math.Min(_inputIndex + 1, _inputBuffer.Count);
                    break;
                default:
                    handleNeeded = true;
                    break;
            }

            if (handleNeeded)
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    var line = CurrentInput;
                    _inputBuffer.Clear();
                    _inputIndex = 0;
                    ClearLine();
                    _inputHistory.Add(line);
                    _readLineLock.Release();
                    return line;
                }
                
                if (key.Key == ConsoleKey.Tab)
                {
                    // TODO: tab complete
                }
                else
                {
                    _inputBuffer.Insert(_inputIndex, key.KeyChar);
                    _inputIndex++;
                }
            }

            DrawPromptLine(inputBufferRenderer: inputBufferRenderer);
        }
    }
}