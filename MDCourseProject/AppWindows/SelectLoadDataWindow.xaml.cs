using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class SelectLoadDataWindow : Window
{
    private bool isJustClosing;

    private string[] filePaths;
    
    public SelectLoadDataWindow()
    {
        InitializeComponent();
        filePaths = new string[6];
    }
    
    protected override void OnClosed(EventArgs e)
    {
        if (!isJustClosing)
            Application.Current.Shutdown();
    }

    private void Button_Continue(object sender, RoutedEventArgs e)
    {
        if (filePaths.Any(string.IsNullOrWhiteSpace))
        {
            MessageBox.Show("Должны быть указаны пути ко всем справочникам!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        try
        {
            MDSystem.clientsSubsystem.LoadFirstCatalogue(filePaths[0]);
            MDSystem.staffSubsystem.LoadFirstCatalogue(filePaths[2]);
            MDSystem.divisionsSubsystem.LoadFirstCatalogue(filePaths[4]);
            
            MDSystem.clientsSubsystem.LoadSecondCatalogue(filePaths[1]);
            MDSystem.staffSubsystem.LoadSecondCatalogue(filePaths[3]);
            MDSystem.divisionsSubsystem.LoadSecondCatalogue(filePaths[5]);
        }
        catch (Exception exception)
        {
            MDDebugConsole.WriteLine(exception.Message);
            MessageBox.Show("Указан некорректный путь!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        isJustClosing = true;
        DialogResult = true;
    }

    private void Button_Back(object sender, RoutedEventArgs e)
    {
        isJustClosing = true;
        DialogResult = false;
    }

    private bool SelectFile(TextBlock textBlock, out string filePath)
    {
        var fileDialog = new OpenFileDialog
        {
            Multiselect = false,
            Filter = "Text files (*.txt)|*.txt",
        };
        
        if (fileDialog.ShowDialog() == true)
        {
            MDDebugConsole.WriteLine(fileDialog.FileName);
            textBlock.Text = fileDialog.FileName;
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            filePath = fileDialog.FileName;
            return true;
        }

        filePath = default;
        return false;
    }

    private void Button_SelectClientDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathClients, out var filePath))
        {
            filePaths[0] = filePath;
        }
    }

    private void Button_SelectAppealDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathAppeals, out var filePath))
        {
            filePaths[1] = filePath;
        }
    }
    
    private void Button_SelectStaffDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathStaff, out var filePath))
        {
            filePaths[2] = filePath;
        }
    }
    
    private void Button_SelectDocumentsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathDocuments, out var filePath))
        {
            filePaths[3] = filePath;
        }
    }
    
    private void Button_SelectDivisionsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathDivisions, out var filePath))
        {
            filePaths[4] = filePath;
        }
    }
    
    private void Button_SelectSendRequestsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathSendRequests, out var filePath))
        {
            filePaths[5] = filePath;
        }
    }
}