using System.Collections.Generic;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class ClientsSubsystem:ISubsystem
{
    public void LoadFirstCatalogue(string filePath)
    {
    }

    public void LoadSecondCatalogue(string filePath)
    {
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