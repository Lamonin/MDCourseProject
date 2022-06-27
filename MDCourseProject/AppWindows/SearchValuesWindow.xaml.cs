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

        _dataAnalyser = MDSystem.Subsystem.Catalogue.BuildSearchValuesWindow(SearchValuesGrid);
    }

    private void Button_AcceptShowSearchResult(object sender, RoutedEventArgs e)
    {
        if (_dataAnalyser is null) return;
        
        MDSystem.Subsystem.Catalogue.Find(MainWindow.Handler.MainDataGrid, _dataAnalyser.GetData());
        MainWindow.Handler.UpdateMainDataGridValues();
        
        Close();
    }
    
    private void Button_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}