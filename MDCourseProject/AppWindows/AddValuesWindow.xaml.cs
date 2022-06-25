﻿using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem;

namespace MDCourseProject.AppWindows;

public partial class AddValuesWindow : Window
{
    private DataAnalyser _dataAnalyser;
    
    public AddValuesWindow()
    {
        InitializeComponent();
        AddValuesInitialize();
    }

    private void AddValuesInitialize()
    {
        AddValuesGrid.ColumnDefinitions.Clear();
        AddValuesGrid.RowDefinitions.Clear();

        _dataAnalyser = MDSystem.Subsystem.BuildAddValuesWindow(AddValuesGrid);
    }

    private void Button_CancelAddValuesWindow(object sender, RoutedEventArgs e)
    {
        Close(); //При отмене просто закрываем окно
    }

    private void Button_AcceptAddValuesWindow(object sender, RoutedEventArgs e)
    {
        //На всякий случай
        if (_dataAnalyser is null) return;

        if (_dataAnalyser.IsCorrectInputData())
        {
            //Логика успешного добавления элементов
            Close();
        }
    }
}