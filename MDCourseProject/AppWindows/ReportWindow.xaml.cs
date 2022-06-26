using System.Windows;

namespace MDCourseProject.AppWindows;

public partial class ReportWindow : Window
{
    public ReportWindow()
    {
        InitializeComponent();
    }
    
    private void Button_AcceptMakeReport(object sender, RoutedEventArgs e)
    {
        
        
        Close();
    }

    private void Button_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}