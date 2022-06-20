using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;

namespace MDCourseProject.AppWindows.WindowsInitializers;

public static class DivisionWindowInitializer
{
    /// <summary> Инициализирует-интерфейс окна добавления подразделений </summary>
    public static DataAnalyser InitializeAddValuesDivisionWindow(Grid mainGrid)
    {
        return new AddValuesDivisionAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:","Тип подразделения:"));
    }
    
    /// <summary> Инициализирует-интерфейс окна добавления отправленных заявок </summary>
    public static DataAnalyser InitializeAddValuesSendRequestsWindow(Grid mainGrid)
    {
        return new AddValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Клиент:", "Телефон клиента:", "Название услуги:", "Дата:"));
    }
}