using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using MDCourseProject.AppWindows;
using MDCourseProject.MDCourseSystem;
using Application = System.Windows.Application;

namespace MDCourseProject
{
    public partial class MainWindow
    {
        public static MainWindow Handler;
        private ObservableCollection<string> cataloguesNames; //Коллекция названий каталогов
        public MainWindow()
        {
            InitializeComponent();
            InitializeMainWindow();
        }

        private void InitializeMainWindow()
        {
            Handler = this;
            
            cataloguesNames = new ObservableCollection<string>();
            ComboBox_Catalogue.ItemsSource = cataloguesNames;
            ComboBox_Subsystem.SelectedIndex = 0;

            var loadDataWindow = new LoadDataWindow();
            loadDataWindow.ShowDialog();
            Loaded += (_, _) => UpdateMainDataGridValues();
        }

        public void UpdateMainDataGridValues()
        {
            if (MainDataGrid.IsLoaded)
            {
                MDSystem.Subsystem.Catalogue?.PrintDataToGrid(MainDataGrid);
                MainDataGrid.Items.Refresh();
            }
        }

        private void ComboBox_OnSubsystemChanged(object sender, SelectionChangedEventArgs e)
        {
            var subsystemComboBox = (ComboBox)sender;
            SelectSubsystem(subsystemComboBox.SelectedIndex);
        }

        private void SelectSubsystem(int index)
        {
            switch (index)
            {
                case 0:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Клиенты");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Clients;
                    break;
                }
                case 1:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Сотрудники");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Staff;
                    break;
                }
                case 2:
                {
                    MDDebugConsole.WriteLine("Активирована подсистема Подразделения");
                    MDSystem.currentSubsystem = SubsystemTypeEnum.Divisions;
                    break;
                }
            }

            if (ComboBox_Catalogue != null)
            {
                cataloguesNames.Clear();
                foreach (var name in MDSystem.Subsystem.CataloguesNames)
                {
                    cataloguesNames.Add(name);
                }
                ComboBox_Catalogue.SelectedIndex = 0;
            }
            
            UpdateMainDataGridValues();
        }

        private void ComboBox_OnCatalogueChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectCatalogueComboBox = (ComboBox)sender;

            //Эта проверка нужна, т.к. индекс бывает отрицательный,
            //потому что кол-во элементов не успело обновиться
            if (selectCatalogueComboBox.SelectedIndex < 0) return;

            MDSystem.Subsystem.CatalogueIndex = selectCatalogueComboBox.SelectedIndex;
            UpdateMainDataGridValues();
        }


        #region СОБЫТИЯ_НАЖАТИЯ_КНОПОК
        private void Button_OpenDebugWindow(object sender, RoutedEventArgs e)
        {
            MDDebugConsole.ShowWindow();
            MDDebugConsole.WriteLine(MDSystem.Subsystem.Catalogue.PrintData());
        }

        private void Button_OpenAddValuesWindow(object sender, RoutedEventArgs e)
        {
            var window = new AddValuesWindow{ Owner = this };
            window.ShowDialog();
        }
        
        private void Button_OpenRemoveValuesWindow(object sender, RoutedEventArgs e)
        {
            var window = new RemoveValuesWindow{ Owner = this };
            window.ShowDialog();
        }
        
        private void Button_OpenSearchValuesWindow(object sender, RoutedEventArgs e)
        {
            var window = new SearchValuesWindow{ Owner = this };
            window.ShowDialog();
        }
        
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_ResetSearchResult(object sender, RoutedEventArgs e)
        {
            UpdateMainDataGridValues();
        }
        
        private void Button_OpenReportWindow(object sender, RoutedEventArgs e)
        {
            var window = new ReportWindow{ Owner = this };
            window.ShowDialog();
        }

        private void Button_SaveCatalogue(object sender, RoutedEventArgs e)
        {
            MDSystem.Subsystem.Catalogue.Save();
        }
    }
}