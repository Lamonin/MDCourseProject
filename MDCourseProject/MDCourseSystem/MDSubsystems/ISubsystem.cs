using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public interface ISubsystem
{
    public DataAnalyser BuildAddValuesWindow(Grid mainGrid);
    public DataAnalyser BuildRemoveValuesWindow(Grid mainGrid);
    public DataAnalyser BuildSearchValuesWindow(Grid mainGrid);
    
    /// <summary> Индекс текущего каталога </summary>
    public int CatalogueIndex { get; set; }
    
    /// <summary> Название текущего каталога </summary>
    public string CurrentCatalogueName { get; }
    
    /// <summary> Названия всех каталогов </summary>
    public IEnumerable<string> CataloguesNames { get; }
}