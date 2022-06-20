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
        mainGrid.ColumnDefinitions.Clear();
        mainGrid.RowDefinitions.Clear();
        
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28),});
        
        //Поле ввода названия подразделения
        var label = new Label { Content = "Название подразделения:" };
        var tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap,
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 0);
        Grid.SetRow(tBox, 0);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);

        //Поле ввода района подразделения
        label = new Label { Content = "Район:"};
        tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap, 
            BorderBrush = new SolidColorBrush(Colors.Black), 
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128,
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 2);
        Grid.SetRow(tBox, 2);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);
        
        //Поле ввода типа подразделения
        label = new Label { Content = "Тип подразделения:"};
        tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap, 
            BorderBrush = new SolidColorBrush(Colors.Black), 
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128,
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 4);
        Grid.SetRow(tBox, 4);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);
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
        
        //Поле ввода названия подразделения
        var label = new Label { Content = "Подразделение:" };
        var tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap,
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 0);
        Grid.SetRow(tBox, 0);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);

        //Поле ввода ФИО клиента
        label = new Label { Content = "Клиент:"};
        tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap, 
            BorderBrush = new SolidColorBrush(Colors.Black), 
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128,
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 2);
        Grid.SetRow(tBox, 2);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);
        
        //Поле ввода названия услуги
        label = new Label { Content = "Название услуги:"};
        tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap, 
            BorderBrush = new SolidColorBrush(Colors.Black), 
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128,
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 4);
        Grid.SetRow(tBox, 4);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);
        
        //Поле ввода даты
        label = new Label { Content = "Услуга:"};
        tBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap, 
            BorderBrush = new SolidColorBrush(Colors.Black), 
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128,
        };

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(tBox);
        
        Grid.SetRow(label, 6);
        Grid.SetRow(tBox, 6);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);
    }
}