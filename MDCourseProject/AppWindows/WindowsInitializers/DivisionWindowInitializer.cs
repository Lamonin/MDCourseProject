using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MDCourseProject.AppWindows.WindowsInitializers;

public static class DivisionWindowInitializer
{
    /// <summary>
    /// Инициализирует-интерфейс окна добавления подразделений
    /// </summary>
    public static void InitializeAddValuesDivisionWindow(Grid mainGrid)
    {
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28),});

        TextBox tBox;
        tBox = CommonWindowGenerator.CreateInputField(mainGrid, "Название подразделения:", 0);
        
        CommonWindowGenerator.CreateInputField(mainGrid,"Район:", 2);
        CommonWindowGenerator.CreateInputField(mainGrid,"Тип подразделения:", 4);
    }
    
    /// <summary>
    /// Инициализирует-интерфейс окна добавления отправленных заявок
    /// </summary>
    public static void InitializeAddValuesSendRequestsWindow(Grid mainGrid)
    {
        mainGrid.ColumnDefinitions.Clear();
        mainGrid.RowDefinitions.Clear();
        
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28),});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28),});
        
        CommonWindowGenerator.CreateInputField(mainGrid,"Подразделение:", 0);
        CommonWindowGenerator.CreateInputField(mainGrid,"Клиент:", 2);
        CommonWindowGenerator.CreateInputField(mainGrid,"Название услуги:", 4);
        CommonWindowGenerator.CreateInputField(mainGrid,"Дата:", 6);
    }
}