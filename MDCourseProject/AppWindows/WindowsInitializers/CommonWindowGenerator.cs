using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MDCourseProject.AppWindows.WindowsInitializers;

public static class CommonWindowGenerator
{

    public static TextBox CreateInputField(Grid mainGrid, string title, int row)
    {
        //Поле ввода названия подразделения
        var label = new Label { Content = title };
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
        
        Grid.SetRow(label, row);
        Grid.SetRow(tBox, row);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(tBox, 1);

        return tBox;
    }
}