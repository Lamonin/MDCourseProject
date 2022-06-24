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
        AddValuesInitialize(MDSystem.currentCatalogue);
    }

    public AddValuesWindow(CatalogueTypeEnum catalogueType) : this()
    {
        AddValuesInitialize(catalogueType);
    }

    private void AddValuesInitialize(CatalogueTypeEnum catalogueType)
    {
        AddValuesGrid.ColumnDefinitions.Clear();
        AddValuesGrid.RowDefinitions.Clear();

        _dataAnalyser = null;

        //Создаем основную разметку окна добавления для указанного каталога
        switch (catalogueType)
        {
            case CatalogueTypeEnum.Clients:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Клиенты");
                _dataAnalyser = ClientsWindowInitializer.InitializeAddValuesClientsWindow(AddValuesGrid);
                break;
            }
            case CatalogueTypeEnum.Appeals:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Обращения");
                _dataAnalyser = ClientsWindowInitializer.InitializeAddValuesAppealsWindow(AddValuesGrid);
                break;
            }
            case CatalogueTypeEnum.Staff:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Сотрудники");
                _dataAnalyser = StaffWindowInitializer.InitializeAddValuesStaffWindow(AddValuesGrid);
                break;
            }
            case CatalogueTypeEnum.Documents:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Документы");
                _dataAnalyser = StaffWindowInitializer.InitializeAddValuesDocumentsWindow(AddValuesGrid);
                break;
            }
            case CatalogueTypeEnum.Divisions:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Подразделения");
                _dataAnalyser = DivisionWindowInitializer.InitializeAddValuesDivisionWindow(AddValuesGrid);
                break;
            }
            case CatalogueTypeEnum.SendRequests:
            {
                Console.Out.WriteLine("Открыто окно добавления в справочник Отправленные Заявки");
                _dataAnalyser = DivisionWindowInitializer.InitializeAddValuesSendRequestsWindow(AddValuesGrid);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
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