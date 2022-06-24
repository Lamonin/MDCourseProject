using System.ComponentModel;
using System.Windows;

namespace MDCourseProject.AppWindows;

public partial class DebugWindow : Window
{
    public DebugWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Visibility = Visibility.Hidden;
        e.Cancel = true;
    }
}