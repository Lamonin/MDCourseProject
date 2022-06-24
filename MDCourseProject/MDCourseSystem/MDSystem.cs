using MDCourseProject.MDCourseSystem.MDCatalogues.Divisions;

namespace MDCourseProject.MDCourseSystem;

public enum SubsystemTypeEnum
{
    Clients,
    Stuff,
    Divisions
}

public enum CatalogueTypeEnum
{
    Clients,
    Appeals,
    Staff,
    Documents,
    Divisions,
    SendRequests
}

public static class MDSystem
{
    /// <summary> Текущая подсистема /// </summary>
    public static SubsystemTypeEnum currentSubsystem = SubsystemTypeEnum.Clients;
    
    /// <summary> Текущий справочник /// </summary>
    public static CatalogueTypeEnum currentCatalogue = CatalogueTypeEnum.Clients;

    public static DivisionsCatalogueHandler divisionsCatalogueHandler;
    
    static MDSystem()
    {
        divisionsCatalogueHandler = new DivisionsCatalogueHandler();
    }
    
}