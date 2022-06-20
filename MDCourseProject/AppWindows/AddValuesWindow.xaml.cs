using System;
using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.AppWindows.WindowsInitializers;

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

        _dataAnalyser = null;
        
        //Создаем основную разметку окна
        switch (MDSystem.currentSubsystem)
        {
            case SubsystemTypeEnum.Clients:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Clients)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Клиенты");
                    _dataAnalyser = ClientsWindowInitializer.InitializeAddValuesClientsWindow(AddValuesGrid);
                }
                else //Обращения
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Обращения");
                    _dataAnalyser = ClientsWindowInitializer.InitializeAddValuesAppealsWindow(AddValuesGrid);
                }
                break;
            case SubsystemTypeEnum.Stuff:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Staff)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Сотрудники");
                    _dataAnalyser = StaffWindowInitializer.InitializeAddValuesStaffWindow(AddValuesGrid);
                }
                else //Документы
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Документы");
                    _dataAnalyser = StaffWindowInitializer.InitializeAddValuesDocumentsWindow(AddValuesGrid);
                }
                break;
            case SubsystemTypeEnum.Divisions:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Divisions)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Подразделения");
                    _dataAnalyser = DivisionWindowInitializer.InitializeAddValuesDivisionWindow(AddValuesGrid);
                }
                else //Отправленные заявки
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Отправленные Заявки");
                    _dataAnalyser = DivisionWindowInitializer.InitializeAddValuesSendRequestsWindow(AddValuesGrid);
                }
                break;
        }
    }

    private void Button_CancelAddValuesWindow(object sender, RoutedEventArgs e)
    {
        Console.Out.WriteLine("Окно добавления закрыто!");
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