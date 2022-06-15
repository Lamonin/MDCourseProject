using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows;

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
            var window = new DebugWindow { Owner = this };
            window.ShowDialog();
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
                    Console.Out.WriteLine("Активирована подсистема Клиенты");
                    currentCatalogue = new ClientsCatalogue();
                    break;
                }
                case 1:
                {
                    Console.Out.WriteLine("Активирована подсистема Сотрудники");
                    currentCatalogue = new StaffCatalogue();
                    break;
                }
                case 2:
                {
                    Console.Out.WriteLine("Активирована подсистема Подразделения");
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
            Console.Out.WriteLine($"Выбран каталог {currentCatalogue.catalogues[comboBox.SelectedIndex]}");
        }
    }

    #region Каталоги
    
    interface ICatalogue
    {
        public List<string> catalogues { get; }
    }
    
    class ClientsCatalogue:ICatalogue
    {
        public List<string> catalogues => new(){"Клиенты", "Обращения"};
    }
    
    class StaffCatalogue:ICatalogue
    {
        public List<string> catalogues => new(){"Сотрудники", "Документы"};
    }
    
    class DivisionsCatalogue:ICatalogue
    {
        public List<string> catalogues => new(){"Подразделения", "Отправленные заявки"};
    }
    
    #endregion
}