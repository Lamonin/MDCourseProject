using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MDCourseProject.AppWindows.WindowsBuilder;

public static class CommonWindowGenerator
{
    public static void GenerateRowsInGrid(Grid grid, int count, int height = 28, int spacing = 8)
    {
        for (int i = 0; i < count; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(height)});
            grid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(spacing)});
        }
    }
    public static TextBox CreateInputField(Grid mainGrid, string title, int row)
    {
        var label = new Label
        {
            Content = title,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };
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
    
    public static TextBox[] CreateInputBetweenField(Grid mainGrid, string title, int row, string fromLabel = "От:", string toLabel = "До:")
    {
        var label = new Label
        {
            Content = title,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        var smallGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto) }, 
                new ColumnDefinition(), 
                new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto) }, 
                new ColumnDefinition()
            }
        };
        
        var labelFrom = new Label
        {
            Content = fromLabel,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        var tBoxFrom = new TextBox
        {
            TextWrapping = TextWrapping.NoWrap,
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxLength = 128
        };
        
        var tBoxTo = new TextBox
        {
            TextWrapping = TextWrapping.NoWrap,
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            MaxLength = 128
        };
        
        var labelTo = new Label
        {
            Content = toLabel,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        smallGrid.Children.Add(labelFrom);
        smallGrid.Children.Add(tBoxFrom);
        smallGrid.Children.Add(labelTo);
        smallGrid.Children.Add(tBoxTo);
        
        Grid.SetColumn(labelFrom, 0);
        Grid.SetColumn(tBoxFrom, 1);
        Grid.SetColumn(labelTo, 2);
        Grid.SetColumn(tBoxTo, 3);
        
        mainGrid.Children.Add(label);
        mainGrid.Children.Add(smallGrid);
        
        Grid.SetRow(label, row);
        Grid.SetRow(smallGrid, row);
        
        Grid.SetColumn(label, 0);
        Grid.SetColumn(smallGrid, 1);
        
        return new []{ tBoxFrom, tBoxTo };
    }

    public static TextBox[] CreateWindow(Grid mainGrid, params string[] titles)
    {
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        
        mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        
        for (var _ = 0; _ < titles.Length; ++_)
        {
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)}); //SPACE
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
        }

        var tBoxes = new TextBox[titles.Length];
        for (var i = 0; i < titles.Length; i++)
        {
            tBoxes[i] = CreateInputField(mainGrid, titles[i], 2*i);
        }
        
        return tBoxes;
    }

    public static void CreateHeadersInDataGrid(DataGrid mainGrid, IEnumerable<string> headers)
    {
        int index = 0;
        foreach (var header in headers)
        {
            mainGrid.Columns[index].Header = header;
            mainGrid.Columns[index].Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            mainGrid.Columns[index].MinWidth = 96;
            index++;
        }
        mainGrid.Columns[index-1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
    }
}