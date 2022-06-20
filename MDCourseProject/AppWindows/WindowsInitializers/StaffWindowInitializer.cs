using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;

namespace MDCourseProject.AppWindows.WindowsInitializers;

public static class StaffWindowInitializer
{
    /// <summary> Инициализирует окна добавления сотрудника </summary>
    public static DataAnalyser InitializeAddValuesStaffWindow(Grid mainGrid)
    {
        return new AddValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность: ", "Район:"));
    }
    
    /// <summary> Инициализирует окна добавления документов </summary>
    public static DataAnalyser InitializeAddValuesDocumentsWindow(Grid mainGrid)
    {
        return new AddValuesDocumentsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:", "Должность: ", "Подразделение:"));
    }
}