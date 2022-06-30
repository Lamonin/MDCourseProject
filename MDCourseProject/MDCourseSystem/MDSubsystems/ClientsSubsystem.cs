using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class ClientsSubsystem:ISubsystem
{
    public Applications _applications { get;}
    public Clients _clients { get;}

    public ClientsSubsystem()
    {
        _applications = new Applications();
        _clients = new Clients();
    }

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

    public Catalogue Catalogue => _catalogueIndex == 0 ? _clients : _applications;

    public IEnumerable<string> CataloguesNames => new []{"Клиенты", "Обращения"};
}