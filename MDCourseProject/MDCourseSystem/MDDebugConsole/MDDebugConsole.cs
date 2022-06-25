using System;
using MDCourseProject.AppWindows;

namespace MDCourseProject.MDCourseSystem;

public static class MDDebugConsole
{
    private static DebugWindow _window;

    private static string _consoleData; //Переменная для хранения вывода
    private static string ConsoleData
    {
        get => _consoleData;
        set
        {
            _consoleData = value;
            
            if (_window.IsVisible) //Если консоль активирована, то обновляем в ней текст
            {
                _window.DebugTextBlock.Text = _consoleData;
            }
        }
    }

    static MDDebugConsole()
    {
        _window = new DebugWindow();
    }

    public static void ShowWindow()
    {
        _window.DebugTextBlock.Text = _consoleData;
        
        if (!_window.IsVisible)
        {
            _window.Show();
            WriteLine("Открыта консоль!");
        }
    }

    public static void Write(string data, bool withTime = false)
    {
        if (withTime)
        {
            _consoleData += $"[{DateTime.Now.ToLongTimeString()}] ";
        }
        ConsoleData += data;
    }

    public static void WriteLine(string data = "", bool withTime = true)
    {
        if (withTime)
        {
            _consoleData += $"[{DateTime.Now.ToLongTimeString()}] ";
        }
        ConsoleData += $"{data}\n";
    }

    public static void Clear()
    {
        _consoleData = String.Empty;
    }
}