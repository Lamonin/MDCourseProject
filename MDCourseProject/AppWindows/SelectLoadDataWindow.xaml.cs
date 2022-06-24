using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using MDCourseProject.MDCourseSystem.MDDebugConsole;

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

    private string SelectFile(TextBlock textBlock)
    {
        var fileDialog = new OpenFileDialog
        {
            Multiselect = false,
            //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Filter = "Text files (*.txt)|*.txt",
        };
        
        if (fileDialog.ShowDialog() == true)
        {
            MDDebugConsole.WriteLine(fileDialog.FileName);
            textBlock.Text = fileDialog.FileName;
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

        return fileDialog.FileName;
    }

    private void Button_SelectClientDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathClients);
    }

    private void Button_SelectAppealDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathAppeals);
    }
    
    private void Button_SelectStaffDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathStaff);
    }
    
    private void Button_SelectDocumentsDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathDocuments);
    }
    
    private void Button_SelectDivisionsDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathDivisions);
    }
    
    private void Button_SelectSendRequestsDataFile(object sender, RoutedEventArgs e)
    {
        var filePath = SelectFile(TextBlock_FilePathSendRequests);
    }
}