using MDCourseProject.MDCourseSystem.MDSubsystems;

namespace MDCourseProject.MDCourseSystem;

public enum SubsystemTypeEnum
{
    Clients,
    Staff,
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
    /// <summary> Текущая подсистема </summary>
    public static SubsystemTypeEnum currentSubsystem = SubsystemTypeEnum.Clients;
    
    /// <summary> Текущий справочник </summary>
    public static CatalogueTypeEnum currentCatalogue = CatalogueTypeEnum.Clients;

    private static ClientsSubsystem _clientsSubsystem;
    private static StaffSubsystem _staffSubsystem;
    private static DivisionsSubsystem _divisionsSubsystem;

    public static ISubsystem Subsystem
    {
        get
        {
            return currentSubsystem switch
            {
                SubsystemTypeEnum.Clients => _clientsSubsystem,
                SubsystemTypeEnum.Staff => _staffSubsystem,
                SubsystemTypeEnum.Divisions => _divisionsSubsystem,
                _ => null //Чтобы не ругался компилятор
            };
        }
    }

    static MDSystem()
    {
        _clientsSubsystem = new ClientsSubsystem();
        _staffSubsystem = new StaffSubsystem();
        _divisionsSubsystem = new DivisionsSubsystem();
    }
}