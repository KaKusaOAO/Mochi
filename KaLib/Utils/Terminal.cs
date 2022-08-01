using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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

    public delegate void InputBufferRendererDelegate(string input, string suggestion, int cursor);
    
    private static SemaphoreSlim _writeLock = new(1, 1);
    private static SemaphoreSlim _readLineLock = new(1, 1);
    private static SemaphoreSlim _promptRenderLock = new(1, 1);
    private static SemaphoreSlim _cursorLock = new(1, 1);

    private static List<char> _inputBuffer = new();
    private static int _suggestApplyFrom = 0;
    private static List<string> _inputHistory = new();
    private static int _historyIndex = -1;
    private static bool _browsingHistory;
    private static int _inputIndex;
    private static IText _currentPrompt;
    private static InputBufferRendererDelegate _inputRenderer;

    private static List<string> _suggestions = new();
    private static int _suggestionIndex = -1;
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

    public static void WriteStdOut(IText text) => WriteStdOut(text.ToAscii());

    public static void WriteLineStdOut(string text)
    {
        _cursorLock.Wait();
        try
        {
            if (Console.WindowTop + Console.WindowHeight - 1 == _stdoutCursorY) _stdoutCursorY--;
            (Console.CursorLeft, Console.CursorTop) = (_stdoutCursorX, _stdoutCursorY);
            Console.WriteLine(text);
            (_stdoutCursorX, _stdoutCursorY) = (Console.CursorLeft, Console.CursorTop);
            ClearRemaining();
            if (Console.WindowTop + Console.WindowHeight - 1 == Console.CursorTop)
            {
                Console.WriteLine();
            }
        }
        finally
        {
            _cursorLock.Release();
        }
        DrawPromptLine(inputBufferRenderer: _inputRenderer);
    }

    public static void WriteLineStdOut(IText text) => WriteLineStdOut(text.ToAscii());
    
    public static void Write(string text)
    {
        _writeLock.Wait();
        Console.Write(text);
        _writeLock.Release();
    }

    public static void Write(IText text) => Write(text.ToAscii());
    
    public static void WriteLine(string text)
    {
        _writeLock.Wait();
        Console.WriteLine(text);
        _writeLock.Release();
    }

    public static string CurrentInput => string.Join("", _inputBuffer);

    public static void WriteLine(IText text) => WriteLine(text.ToAscii());

    public static void ClearLine()
    {
        Write("\u001b[2K");
        Console.CursorLeft = 0;
    }

    public static void ClearRemaining()
    {
        // Console.Write(" ".PadRight(Console.BufferWidth - 1 - Console.CursorLeft));
        Console.Write("\u001b[J");
    }
    
    public static void DrawPromptLine(string text = null, InputBufferRendererDelegate inputBufferRenderer = null)
    {
        _promptRenderLock.Wait();
        _cursorLock.Wait();

        try
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

            var line = text ?? (_browsingHistory && _historyIndex >= 0
                ? _inputHistory[_inputHistory.Count - _historyIndex - 1]
                : CurrentInput);

            inputBufferRenderer ??= (str, _, _) =>
            {
                Console.Write(str);
                Console.Write(" ".PadRight(Console.BufferWidth - 1 - Console.CursorLeft));
            };

            var suggesting = "";
            if (_suggestions.Any() && _suggestionIndex < 0) _suggestionIndex = 0;
            if (_suggestionIndex >= 0)
            {
                try
                {
                    suggesting = _suggestions[_suggestionIndex];
                }
                catch (ArgumentOutOfRangeException)
                {
                    suggesting = "";
                }
            }

            inputBufferRenderer(line, suggesting, _inputIndex);

            Console.CursorLeft = _browsingHistory ? c + line.Length : c + _inputIndex;
        }
        finally
        {
            if (_cursorLock.CurrentCount == 0) _cursorLock.Release();
            if (_promptRenderLock.CurrentCount == 0) _promptRenderLock.Release();
        }
    }

    public static string ReadLine(string prompt, AutoCompleterDelegate autoCompleter = null, InputBufferRendererDelegate inputBufferRenderer = null) =>
        ReadLine(LiteralText.Of(prompt), autoCompleter, inputBufferRenderer);

    private static void UpdateSuggestions(AutoCompleterDelegate autoCompleter,
        InputBufferRendererDelegate inputBufferRenderer = null)
    {
        Task.Run(async () =>
        {
            await Task.Yield();
            _suggestions.Clear();
            _suggestions.AddRange(autoCompleter(CurrentInput, _inputIndex));
            
            if (_suggestions.Any()) _suggestionIndex %= _suggestions.Count;
            else _suggestionIndex = -1;
            DrawPromptLine(inputBufferRenderer: inputBufferRenderer);
        });
    }
    
    public static string ReadLine(IText prompt = null, AutoCompleterDelegate autoCompleter = null, InputBufferRendererDelegate inputBufferRenderer = null)
    {
        _readLineLock.Wait();
        try
        {
            _currentPrompt = prompt;
            _inputRenderer = inputBufferRenderer;
            DrawPromptLine(inputBufferRenderer: inputBufferRenderer);
            UpdateSuggestions(autoCompleter);

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

                    if (_historyIndex == -1)
                    {
                        _browsingHistory = false;
                        _inputIndex = _inputBuffer.Count;
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
                var isSuggestion = false;
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
                        _inputRenderer = null;
                        return line;
                    }

                    if (key.Key == ConsoleKey.Tab)
                    {
                        _suggestionIndex += key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? _suggestions.Count - 1 : 1;
                        if (_suggestions.Any()) _suggestionIndex %= _suggestions.Count;
                        else _suggestionIndex = -1;
                    }
                    else
                    {
                        _inputBuffer.Insert(_inputIndex, key.KeyChar);
                        _inputIndex++;
                    }
                }

                UpdateSuggestions(autoCompleter, inputBufferRenderer);
                DrawPromptLine(inputBufferRenderer: inputBufferRenderer);
            }
        }
        finally
        {
            _readLineLock.Release();
        }
    }
}