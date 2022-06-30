using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class ClientsSubsystem:ISubsystem
{
    public Applications _applications { get; set; }
    public Clients _clients { get; set; }

    public void LoadDefaultFirstCatalogue()
    {
        //LoadFirstCatalogue("DefaultFiles/имя_справочника.txt");
    }

    public void LoadDefaultSecondCatalogue()
    {
        //LoadSecondCatalogue("DefaultFiles/имя_справочника.txt");
    }
    
    public void LoadFirstCatalogue(string filePath)
    {
    }

    public void LoadSecondCatalogue(string filePath)
    {
    }

    public bool MakeReport(string[] data)
    {
        return false;
    }

    public DataAnalyser BuildReportWindow(Grid mainGrid)
    {
        return null;
    }

    private int _catalogueIndex;
    public int CatalogueIndex
    {
        get => _catalogueIndex;
        set
        {
            if (value < 0) value = 0;
            _catalogueIndex = value;
        }
    }

    public Catalogue Catalogue => null;

    public IEnumerable<string> CataloguesNames => new []{"Клиенты", "Обращения"};
}