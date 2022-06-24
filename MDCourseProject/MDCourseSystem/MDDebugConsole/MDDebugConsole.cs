using System;
using MDCourseProject.AppWindows;

namespace MDCourseProject.MDCourseSystem.MDDebugConsole;

public static class MDDebugConsole
{
    private static DebugWindow _window;
    private static string _consoleData;

    static MDDebugConsole()
    {
        _window = new DebugWindow();
    }

    public static void ShowWindow()
    {
        _window.DebugTextBlock.Text = _consoleData;
        _window.ShowDialog();
    }

    public static void Write(string data, bool withTime = false)
    {
        if (withTime)
        {
            _consoleData += $"[{DateTime.Now.ToLongTimeString()}] ";
        }
        _consoleData += data;
    }

    public static void WriteLine(string data = "", bool withTime = true)
    {
        if (withTime)
        {
            _consoleData += $"[{DateTime.Now.ToLongTimeString()}] ";
        }
        _consoleData += $"{data}\n";
    }

    public static void Clear()
    {
        _consoleData = String.Empty;
    }
}