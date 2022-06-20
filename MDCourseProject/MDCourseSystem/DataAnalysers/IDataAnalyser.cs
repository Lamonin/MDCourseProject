using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public interface IDataAnalyser
{
    public string[] GetData();
    public bool IsCorrectInputData();
}