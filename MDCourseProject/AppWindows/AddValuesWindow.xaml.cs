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
        //Создаем основную разметку окна
        switch (MDSystem.currentSubsystem)
        {
            case SubsystemTypeEnum.Clients:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Clients)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Клиенты");
                }
                else //Обращения
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Обращения");
                }
                break;
            case SubsystemTypeEnum.Stuff:
                if (MDSystem.currentCatalogue == CatalogueTypeEnum.Staff)
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Сотрудники");
                }
                else //Документы
                {
                    Console.Out.WriteLine("Открыто окно добавления в справочник Документы");
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