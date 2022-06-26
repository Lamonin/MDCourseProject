using System;
using System.Windows;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class LoadDataWindow : Window
{
    private bool isJustClosing;
    public LoadDataWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (!isJustClosing)
            Application.Current.Shutdown();
    }

    private void Button_LoadDefaultData(object sender, RoutedEventArgs e)
    {
        //Загрузка каталогов по умолчанию
        MDSystem.divisionsSubsystem.LoadDefaultFirstCatalogue();
        MDSystem.divisionsSubsystem.LoadDefaultSecondCatalogue();
        
        isJustClosing = true;
        DialogResult = true;
    }

    private void Button_LoadUserData(object sender, RoutedEventArgs e)
    {
        isJustClosing = true;
        
        var loadUserDataWindow = new SelectLoadDataWindow { Owner = this };
        if (loadUserDataWindow.ShowDialog() == true)
        {
            DialogResult = true;
        }
        else
        {
            isJustClosing = false;
        }
    }
}