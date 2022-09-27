using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using KaLib.IO.Controllers.DualSense;
using KaLib.Structs;

namespace KaLib.IO.Controllers.Test;

public static class ControllerGui
{
    private const float HistorySustainSec = 0.25f;
    private static readonly Dictionary<int, List<TimedRecord<Vector2>>> _stickHistoryMap = new();
    private static readonly List<TimedRecord<TouchState>> _touch1History = new();
    private static readonly List<TimedRecord<TouchState>> _touch2History = new();

    private const float PlotSustainSec = 1f;
    private static readonly Dictionary<int, List<TimedRecord<float>>> _plotMap = new();

    private record TimedRecord<T>(T Entry)
    {
        public DateTime Time { get; } = DateTime.Now;
    }

    public static void DrawStick(int id, Vector2 vector, bool pressed = false)
    {
        var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        var drawList = ImGui.GetWindowDrawList();
        var center = new Vector2(20, 20);
        var ga = ImGui.GetStyle().Alpha;
        
        void DrawStickHistory(List<TimedRecord<Vector2>> history)
        {
            var prevPoint = pos + center;
            foreach (var t in history)
            {
                var alpha = (uint)(Math.Clamp(1 - (DateTime.Now - t.Time).TotalSeconds / HistorySustainSec, 0, 1) * 255);
                alpha = (uint) (alpha * ga);
                var col = alpha << 24 | 0xffcc00;
                var v = pos + center + t.Entry * 20;
                drawList.AddLine(prevPoint, v, col);
                prevPoint = v;
            }

            history.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds > HistorySustainSec);
        }

        if (!_stickHistoryMap.ContainsKey(id))
        {
            _stickHistoryMap.Add(id, new List<TimedRecord<Vector2>>());
        }

        var history = _stickHistoryMap[id];
        history.Add(new TimedRecord<Vector2>(vector));

        var ca = (uint) (0xff * ga) << 24;
        drawList.AddCircle(pos + center, 20, pressed ? ca | 0xffffff : ca | 0x888888);
        DrawStickHistory(history);
        drawList.AddCircleFilled(pos + center + vector * 20, 2, ca | 0xffffff);
        ImGui.Dummy(new Vector2(40, 40));
    }

    public static void DrawTouchPad(DualSenseTouchPad.TouchPadState state, Color? color = null)
    {
        var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        var drawList = ImGui.GetWindowDrawList();
        var tWidth = 120f / 9 * 16;
        var tHeight = 120f;
        color ??= Color.White;

        var ga = ImGui.GetStyle().Alpha;

        void DrawTouchHistory(List<TimedRecord<TouchState>> history)
        {
            var prevTouch = new TouchState();
            var prevPoint = pos;
        
            foreach (var r in history)
            {
                var t = r.Entry;
                var alpha = t.IsActive && prevTouch.IsActive ? (uint)(Math.Clamp(1 - (DateTime.Now - r.Time).TotalSeconds / HistorySustainSec, 0, 1) * 255) : 0;
                alpha = (uint) (alpha * ga);
                var col = alpha << 24 | 0xffcc00;
                var v = pos + t.Position * new Vector2(tWidth, tHeight);
                drawList.AddLine(prevPoint, v, col);
                prevPoint = v;
                prevTouch = t;
            }

            history.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds > HistorySustainSec);
        }
        
        void DrawTouch(TouchState tState, uint tColor)
        {
            if (!tState.IsActive) return;
        
            var tPos = tState.Position * new Vector2(tWidth, tHeight);
            var alpha = (uint) (0x88 * ga) << 24;
            drawList.AddLine(pos + tPos with { Y = 0 }, pos + tPos with { Y = tHeight }, alpha | tColor);
            drawList.AddLine(pos + tPos with { X = 0 }, pos + tPos with { X = tWidth }, alpha | tColor);
            drawList.AddText(pos + tPos + new Vector2(5, 3), 0xff000000 | tColor, $"#{tState.Id}");
        }

        var c = color.Value;
        var border = (uint) (c.B << 16 | c.G << 8 | c.R);
        var bAlpha = (uint) ((state.Pressed ? 0xff : 0x88) * ga);

        drawList.AddRect(pos, pos + new Vector2(tWidth, tHeight), bAlpha << 24 | border);
        if (state.TouchStates != null)
        {
            _touch1History.Add(new TimedRecord<TouchState>(state.TouchStates[0]));
            _touch2History.Add(new TimedRecord<TouchState>(state.TouchStates[1]));

            DrawTouchHistory(_touch1History);
            DrawTouchHistory(_touch2History);
            DrawTouch(state.TouchStates[0], 0xaaaaff);
            DrawTouch(state.TouchStates[1], 0xffffaa);
        }

        ImGui.Dummy(new Vector2(tWidth, tHeight));
    }
    
    public static void DrawPlotFloat(int id, float value, float? maxValue = null)
    {
        var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        var drawList = ImGui.GetWindowDrawList();
        var pWidth = 90f;
        var pHeight = 30f;
        var alpha = (uint) (0xff * ImGui.GetStyle().Alpha) << 24;

        void DrawHistory(List<TimedRecord<float>> history)
        {
            var maxVal = maxValue ?? history.Max(x => x.Entry);

            Vector2 MakePoint(TimedRecord<float> r)
            {
                var x = Math.Clamp(1 - (DateTime.Now - r.Time).TotalSeconds / PlotSustainSec, 0, 1);
                return new Vector2((float) x, 1 - r.Entry / maxVal);
            }
            
            var prevPoint = pos + MakePoint(history.First()) * new Vector2(pWidth, pHeight);
            var col = alpha | 0xffcc00;
            
            foreach (var t in history)
            {
                var v = pos + MakePoint(t) * new Vector2(pWidth, pHeight);
                drawList.AddLine(prevPoint, v, col);
                prevPoint = v;
            }

            history.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds > PlotSustainSec);
        }

        if (!_plotMap.ContainsKey(id))
        {
            _plotMap.Add(id, new List<TimedRecord<float>>());
        }

        var history = _plotMap[id];
        history.Add(new TimedRecord<float>(value));
        
        DrawHistory(history);
        ImGui.Dummy(new Vector2(pWidth, pHeight));
    }

    public static void DrawPlotFloatNormalized(float[] values)
    {
        var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        var drawList = ImGui.GetWindowDrawList();
        var pWidth = 180f;
        var pHeight = 30f;
        var alpha = (uint) (0xff * ImGui.GetStyle().Alpha) << 24;
        var col = alpha | 0xffcc00;

        var prevPoint = pos + new Vector2(0, pHeight / 2);
        for (var i = 0f; i < values.Length; i += values.Length / pWidth / 2)
        {
            var val = values[(int)i];
            var v = pos + new Vector2(i * 1f / values.Length, 1 - (val / 2 + 0.5f)) * new Vector2(pWidth, pHeight);
            drawList.AddLine(prevPoint, v, col);
            prevPoint = v;
        }
        
        ImGui.Dummy(new Vector2(pWidth, pHeight));
    }
    
    public static void Update()
    {
        
    }
}