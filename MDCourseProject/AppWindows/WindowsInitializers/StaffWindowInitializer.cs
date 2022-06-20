using System.Windows.Controls;

namespace MDCourseProject.AppWindows.WindowsInitializers;

public static class StaffWindowInitializer
{
    /// <summary>
    /// Инициализирует окна добавления сотрудника
    /// </summary>
    public static void InitializeAddValuesStaffWindow(Grid mainGrid)
    {
        CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность: ", "Район:");
    }
    
    /// <summary>
    /// Инициализирует окна добавления документов
    /// </summary>
    public static void InitializeAddValuesDocumentsWindow(Grid mainGrid)
    {
        CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:", "Должность: ", "Подразделение:");
    }
}