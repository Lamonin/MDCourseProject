using System.Collections.Generic;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public interface ISubsystem
{
    public void LoadFirstCatalogue(string filePath);
    public void LoadSecondCatalogue(string filePath);
    
    /// <summary> Индекс текущего каталога </summary>
    public int CatalogueIndex { get; set; }

    public Catalogue Catalogue { get; }

    /// <summary> Названия всех каталогов </summary>
    public IEnumerable<string> CataloguesNames { get; }
}