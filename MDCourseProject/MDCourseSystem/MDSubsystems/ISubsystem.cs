using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public interface ISubsystem
{
    public void LoadFirstCatalogue(string filePath);
    public void LoadSecondCatalogue(string filePath);

    public bool MakeReport(string[] data);

    public DataAnalyser BuildReportWindow(Grid mainGrid);

    /// <summary> Индекс текущего каталога </summary>
    public int CatalogueIndex { get; set; }

    public Catalogue Catalogue { get; }

    /// <summary> Названия всех каталогов </summary>
    public IEnumerable<string> CataloguesNames { get; }
}