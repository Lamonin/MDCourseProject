using MDCourseProject.MDCourseSystem.MDSubsystems;

namespace MDCourseProject.MDCourseSystem;

public enum SubsystemTypeEnum
{
    Clients,
    Staff,
    Divisions
}

public static class MDSystem
{
    /// <summary> Текущая подсистема </summary>
    public static SubsystemTypeEnum currentSubsystem = SubsystemTypeEnum.Clients;

    public static readonly ClientsSubsystem clientsSubsystem;
    public static readonly StaffSubsystem staffSubsystem;
    public static readonly DivisionsSubsystem divisionsSubsystem;

    public static ISubsystem Subsystem
    {
        get
        {
            return currentSubsystem switch
            {
                SubsystemTypeEnum.Clients => clientsSubsystem,
                SubsystemTypeEnum.Staff => staffSubsystem,
                SubsystemTypeEnum.Divisions => divisionsSubsystem,
                _ => null //Чтобы не ругался компилятор
            };
        }
    }

    static MDSystem()
    {
        clientsSubsystem = new ClientsSubsystem();
        staffSubsystem = new StaffSubsystem();
        divisionsSubsystem = new DivisionsSubsystem();
        MDDebugConsole.WriteLine("Исходный код доступен по ссылке: https://github.com/Lamonin/MDCourseProject");
    }
}