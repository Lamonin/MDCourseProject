using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.MDCourseSystem.MDDebugConsole;

namespace MDCourseProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            cataloguesNames = new ObservableCollection<string>();
            ComboBox_Catalogue.ItemsSource = cataloguesNames;
            ComboBox_Subsystem.SelectedIndex = 0;
        }
        
        private void DebugButtonClick(object sender, RoutedEventArgs e)
        {
            MDDebugConsole.WriteLine("Открыта консоль!");
            MDDebugConsole.ShowWindow();
        }

        private void Selector_OnSubsystemChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            SelectSubsystem(comboBox.SelectedIndex);
        }

        private void SelectSubsystem(int index)
        {
            switch (index)
            {
                case 0:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Клиенты");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Clients;
                    currentCatalogue = new ClientsCatalogue();
                    break;
                }
                case 1:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Сотрудники");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Stuff;
                    currentCatalogue = new StaffCatalogue();
                    break;
                }
                case 2:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Подразделения");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Divisions;
                    currentCatalogue = new DivisionsCatalogue();
                    break;
                }
            }
            
            if (ComboBox_Catalogue != null)
            {
                cataloguesNames.Clear();
                currentCatalogue.catalogues.ForEach(s => cataloguesNames.Add(s));
                ComboBox_Catalogue.SelectedIndex = 0;
            }
        }

        private ICatalogue currentCatalogue;
        private ObservableCollection<string> cataloguesNames; //Коллекция названий каталогов

        private void Selector_OnCatalogueChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;

            //Эта проверка нужна, т.к. индекс бывает отрицательный,
            //потому что кол-во элементов не успело обновиться
            if (comboBox.SelectedIndex < 0) return;
            
            currentCatalogue.SetCatalogue(comboBox.SelectedIndex);
            Console.Out.WriteLine($"Выбран каталог {currentCatalogue.catalogues[comboBox.SelectedIndex]}");
        }

        private void Button_OpenAddValuesWindow(object sender, RoutedEventArgs e)
        {
            var window = new AddValuesWindow();
            window.ShowDialog();
        }
    }
}