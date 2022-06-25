using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDSubsystems
{
    public struct Division:IComparable<Division>
    {
        public Division(string name, string area, string type)
        {
            Name = name;
            Area = area;
            Type = type;
        }
        
        public int CompareTo(Division other)
        {
            var compareRes = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            compareRes = string.Compare(Area, other.Area, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            return string.Compare(Type, other.Type, StringComparison.OrdinalIgnoreCase);
        }
        
        public override string ToString()
        {
            return $"{Name} ({Area})";
        }
        
        public string Name {get; set;}
        public string Area {get; set;}
        public string Type {get; set;}
    }
    
    public struct SendRequest:IComparable<SendRequest>
    {
        public SendRequest(string division, string client, string service, string date)
        {
            Division = division;
            Client = client;
            Service = service;
            Date = date;
        }

        public int CompareTo(SendRequest other)
        {
            var compareRes = String.Compare(Division, other.Division, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes; 
            
            compareRes = string.Compare(Client, other.Client, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            compareRes = string.Compare(Service, other.Service, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            return string.Compare(Date, other.Date, StringComparison.OrdinalIgnoreCase);
        }

        public string Division { get; set; }
        public string Client { get; set; }
        public string Service { get; set; }
        public string Date { get; set; }
    }

    public readonly struct DivisionNameAndArea:IComparable<DivisionNameAndArea>
    { 
        public DivisionNameAndArea(string name, string area)
        {
            Name = name;
            Area = area;
        }

        public int CompareTo(DivisionNameAndArea other)
        {
            var nameComparison = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (nameComparison != 0) return nameComparison;
            
            return string.Compare(Area, other.Area, StringComparison.OrdinalIgnoreCase);
        }
        
        public readonly string Name;
        public readonly string Area;
    }

    public readonly struct SendRequestClientServiceAndDate:IComparable<SendRequestClientServiceAndDate>
    {
        public SendRequestClientServiceAndDate(string client, string service, string date)
        {
            Client = client;
            Service = service;
            Date = date;
        }

        public int CompareTo(SendRequestClientServiceAndDate other)
        {
            var compareRes = string.Compare(Client, other.Client, StringComparison.Ordinal);
            if (compareRes != 0) return compareRes;

            compareRes = string.Compare(Service, other.Service, StringComparison.Ordinal);
            if (compareRes != 0) return compareRes;
            
            return string.Compare(Date, other.Date, StringComparison.Ordinal);
        }
        
        public readonly string Client;
        public readonly string Service;
        public readonly string Date;
    }
    
    public class DivisionsSubsystem:ISubsystem
    {
        private int _catalogueIndex;
        
        public DivisionsSubsystem()
        {
            DivisionsTable = new StaticHashTable<DivisionNameAndArea, string>(1000);
            SendRequestsTree = new LRBTree<DivisionNameAndArea, SendRequestClientServiceAndDate>();
            CatalogueIndex = 0;

            _divisionsData = new ObservableCollection<Division>();
            _sendRequestsData = new ObservableCollection<SendRequest>();
        }
        
        public StaticHashTable<DivisionNameAndArea, string> DivisionsTable { get; private set; }
        public LRBTree<DivisionNameAndArea, SendRequestClientServiceAndDate> SendRequestsTree { get; private set; }

        private ObservableCollection<Division> _divisionsData;
        private ObservableCollection<SendRequest> _sendRequestsData;
        
        public void Add(string[] data)
        {
            if (CatalogueIndex == 0)
            {
                var key = new DivisionNameAndArea(data[0], data[1]);
                DivisionsTable.Add(key, data[2]);
                
                _divisionsData.Add(new Division(key.Name, key.Area, data[2]));
            }
            else
            {
                var key = new DivisionNameAndArea(data[0], data[1]);
                var value = new SendRequestClientServiceAndDate(data[1], data[2], data[3]);
                SendRequestsTree.Add(key, value);
                
                _sendRequestsData.Add(new SendRequest(data[0], data[1], data[2], data[3]));
            }
            
            OnCatalogueValuesUpdated?.Invoke();
        }

        public void PrintDataInGrid(DataGrid mainDataGrid)
        {
            mainDataGrid.ItemsSource = null;
            mainDataGrid.Columns.Clear();
            
            if (CatalogueIndex == 0)
            {
                mainDataGrid.ItemsSource = _divisionsData;
                
                //СТРОИМ НАИМЕНОВАНИЕ СТОЛБЦОВ
                mainDataGrid.Columns[0].Header = "Подразделение";
                mainDataGrid.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                
                mainDataGrid.Columns[1].Header = "Район";
                mainDataGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                
                mainDataGrid.Columns[2].Header = "Тип подразделения";
                mainDataGrid.Columns[2].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
            else
            {
                mainDataGrid.ItemsSource = _sendRequestsData;
                
                //СТРОИМ НАИМЕНОВАНИЕ СТОЛБЦОВ
                mainDataGrid.Columns[0].Header = "Подразделение";
                mainDataGrid.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                
                mainDataGrid.Columns[1].Header = "Клиент";
                mainDataGrid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                
                mainDataGrid.Columns[2].Header = "Услуга";
                mainDataGrid.Columns[2].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                
                mainDataGrid.Columns[3].Header = "Дата";
                mainDataGrid.Columns[3].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        public DataAnalyser BuildAddValuesWindow(Grid mainGrid)
        {
            if (CatalogueIndex == 0)
                return new AddValuesDivisionAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:", "Тип подразделения:"));

            return new AddValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
        }

        public DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            if (CatalogueIndex == 0)
                return new RemoveValuesDivisionsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Район:"));

            return new RemoveValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
        }

        public DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            if (CatalogueIndex == 0)
            {
                return new SearchValuesDivisionsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:", "Тип подразделения:"));
            }

            return new SearchValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
        }

        public event Action OnCatalogueValuesUpdated;

        public int CatalogueIndex
        {
            get => _catalogueIndex;
            set
            {
                if (value < 0) value = 0;
                _catalogueIndex = value;

                //Этот вывод в консоль так... для красоты)
                if (_catalogueIndex == 0)
                    MDDebugConsole.WriteLine("Выбран справочник Подразделения");
                else
                    MDDebugConsole.WriteLine("Выбран справочник Отправленные заявки");
            }
        }
        
        public string CurrentCatalogueName => CatalogueIndex == 0 ? "Подразделения" : "Отправленные заявки";
        public IEnumerable<string> CataloguesNames => new[]{"Подразделения", "Отправленные заявки"};
    }
}


