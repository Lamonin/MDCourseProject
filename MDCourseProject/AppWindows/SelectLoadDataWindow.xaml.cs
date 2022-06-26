using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class SelectLoadDataWindow : Window
{
    private bool isJustClosing;
    public SelectLoadDataWindow()
    {
        InitializeComponent();
    }
    
    protected override void OnClosed(EventArgs e)
    {
        if (!isJustClosing)
            Application.Current.Shutdown();
    }

    private void Button_Continue(object sender, RoutedEventArgs e)
    {
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
            MDSystem.clientsSubsystem.LoadFirstCatalogue(filePath);
        }
    }

    private void Button_SelectAppealDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathAppeals, out var filePath))
        {
            MDSystem.clientsSubsystem.LoadSecondCatalogue(filePath);
        }
    }
    
    private void Button_SelectStaffDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathStaff, out var filePath))
        {
            MDSystem.staffSubsystem.LoadFirstCatalogue(filePath);
        }
    }
    
    private void Button_SelectDocumentsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathDocuments, out var filePath))
        {
            MDSystem.staffSubsystem.LoadSecondCatalogue(filePath);
        }
    }
    
    private void Button_SelectDivisionsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathDivisions, out var filePath))
        {
            MDSystem.divisionsSubsystem.LoadFirstCatalogue(filePath);
        }
    }
    
    private void Button_SelectSendRequestsDataFile(object sender, RoutedEventArgs e)
    {
        if (SelectFile(TextBlock_FilePathSendRequests, out var filePath))
        {
            MDSystem.divisionsSubsystem.LoadSecondCatalogue(filePath);
        }
    }
}