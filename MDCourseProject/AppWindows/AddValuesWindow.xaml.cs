using System;
using System.Windows;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.AppWindows.WindowsInitializers;

namespace MDCourseProject.AppWindows;

public partial class AddValuesWindow : Window
{
    public AddValuesWindow()
    {
        InitializeComponent();
        AddValuesInitialize();
    }

    private void AddValuesInitialize()
    {
        AddValuesGrid.ColumnDefinitions.Clear();
        AddValuesGrid.RowDefinitions.Clear();
        
        //Создаем основную разметку окна
        switch (MDSystem.currentSubsystem)
        {
            case SubsystemTypeEnum.Clients:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Clients)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Клиенты");
                    ClientsWindowInitializer.InitializeAddValuesClientsWindow(AddValuesGrid);
                }
                else //Обращения
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Обращения");
                    ClientsWindowInitializer.InitializeAddValuesAppealsWindow(AddValuesGrid);
                }
                break;
            case SubsystemTypeEnum.Stuff:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Staff)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Сотрудники");
                    StaffWindowInitializer.InitializeAddValuesStaffWindow(AddValuesGrid);
                }
                else //Документы
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Документы");
                    StaffWindowInitializer.InitializeAddValuesDocumentsWindow(AddValuesGrid);
                }
                break;
            case SubsystemTypeEnum.Divisions:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Divisions)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Подразделения");
                    DivisionWindowInitializer.InitializeAddValuesDivisionWindow(AddValuesGrid);
                }
                else //Отправленные заявки
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Отправленные Заявки");
                    DivisionWindowInitializer.InitializeAddValuesSendRequestsWindow(AddValuesGrid);
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
        //Логика обработки добавления
        Close();
    }
}