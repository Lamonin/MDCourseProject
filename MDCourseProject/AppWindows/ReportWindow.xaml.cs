using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class ReportWindow : Window
{
    private DataAnalyser _dataAnalyser;
    public ReportWindow()
    {
        InitializeComponent();
        Initialize();
    }

    private void Initialize()
    {
        _dataAnalyser = MDSystem.Subsystem.BuildReportWindow(ReportValuesGrid);
    }
    
    private void Button_AcceptMakeReport(object sender, RoutedEventArgs e)
    {
        if (!_dataAnalyser.IsCorrectInputData()) return;
        
        if (!MDSystem.Subsystem.MakeReport(_dataAnalyser.GetData()))
        {
            //Диалоговое окно сохранения отчета было просто закрыто
            return;
        }
        
        Close();
    }

    private void Button_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}