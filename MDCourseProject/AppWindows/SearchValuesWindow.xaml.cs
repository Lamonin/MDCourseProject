using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class SearchValuesWindow : Window
{
    private DataAnalyser _dataAnalyser;
    public SearchValuesWindow()
    {
        InitializeComponent();
        SearchValuesInitialize();
    }
    
    private void SearchValuesInitialize()
    {
        SearchValuesGrid.ColumnDefinitions.Clear();
        SearchValuesGrid.RowDefinitions.Clear();

        _dataAnalyser = MDSystem.Subsystem.BuildSearchValuesWindow(SearchValuesGrid);
    }

    private void Button_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}