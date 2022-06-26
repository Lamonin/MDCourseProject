using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class RemoveValuesWindow : Window
{
    private DataAnalyser _dataAnalyser;
    public RemoveValuesWindow()
    {
        InitializeComponent();
        RemoveValuesInitialize();
    }

    private void RemoveValuesInitialize()
    {
        RemoveValuesGrid.ColumnDefinitions.Clear();
        RemoveValuesGrid.RowDefinitions.Clear();

        _dataAnalyser = MDSystem.Subsystem.BuildRemoveValuesWindow(RemoveValuesGrid);
    }
    
    private void Button_AcceptRemoveValues(object sender, RoutedEventArgs e)
    {
        MDSystem.Subsystem.Remove(_dataAnalyser.GetData());
        Close();
    }

    private void Button_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}