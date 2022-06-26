using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using Microsoft.Win32;

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
        public SendRequest(string divisionName, string client, string service, string date)
        {
            DivisionName = divisionName;
            Client = client;
            Service = service;
            Date = date;
        }

        public int CompareTo(SendRequest other)
        {
            var compareRes = String.Compare(DivisionName, other.DivisionName, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes; 
            
            compareRes = string.Compare(Client, other.Client, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            compareRes = string.Compare(Service, other.Service, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            return string.Compare(Date, other.Date, StringComparison.OrdinalIgnoreCase);
        }

        public string DivisionName { get; set; }
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

        public override string ToString()
        {
            return Name + "; " + Area;
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
            CatalogueIndex = 0;
            
            DivisionsTable = new StaticHashTable<DivisionNameAndArea, string>(1000);
            SendRequestsTree = new LRBTree<DivisionNameAndArea, SendRequestClientServiceAndDate>();

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
        }

        public void Remove(string[] data)
        {
            if (CatalogueIndex == 0)
            {
                var key = new DivisionNameAndArea(data[0], data[1]);
                if (DivisionsTable.TryGetValue(key, out var value))
                {
                    DivisionsTable.Remove(key, value);
                    _divisionsData.Remove(new Division(key.Name, key.Area, value));
                }
            }
            else
            {
                var key = new DivisionNameAndArea(data[0], data[1]);
                var value = new SendRequestClientServiceAndDate(data[1], data[2], data[3]);
                SendRequestsTree.Remove(key, value);

                _sendRequestsData.Remove(new SendRequest(data[0], data[1], data[2], data[3]));
            }
        }
        
        public void Find(DataGrid mainDataGrid, string[] data)
        {
            mainDataGrid.ItemsSource = null;
            
            string[] headers;
            if (CatalogueIndex == 0)
            {
                var searchResult = new ObservableCollection<Division>();
                if (DivisionsTable.TryGetValue(new DivisionNameAndArea(data[0], data[1]), out var value))
                {
                    searchResult.Add(new Division(data[0], data[1], value));
                }
                mainDataGrid.ItemsSource = searchResult;
                
                headers = new []{"Подразделение", "Район", "Тип подразделения"};
            }
            else
            {
                headers = new []{"Подразделение", "Клиент", "Услуга", "Дата"};
            }
            CommonWindowGenerator.CreateHeadersInDataGrid(mainDataGrid, headers);
        }

        public void PrintDataInGrid(DataGrid mainDataGrid)
        {
            mainDataGrid.ItemsSource = null;
            mainDataGrid.Columns.Clear();

            string[] headers;
            
            if (CatalogueIndex == 0)
            {
                mainDataGrid.ItemsSource = _divisionsData;
                headers = new []{"Подразделение", "Район", "Тип подразделения"};
            }
            else
            {
                mainDataGrid.ItemsSource = _sendRequestsData;
                headers = new []{"Подразделение", "Клиент", "Услуга", "Дата"};
            }
            
            //УСТАНАВЛИВАЕТ НАЗВАНИЯ СТОЛБЦОВ
            CommonWindowGenerator.CreateHeadersInDataGrid(mainDataGrid, headers);
        }

        public void LoadDefaultFirstCatalogue()
        {
            LoadFirstCatalogue("DefaultFiles/divisions_default.txt");
        }
        
        public void LoadFirstCatalogue(string filePath)
        {
            var reader = new StreamReader(filePath);
            
            CatalogueIndex = 0;

            while (!reader.EndOfStream)
            {
                var data = reader.ReadLine()!.Split(';');
                Add(data);
            }
            
            reader.Close();
        }

        public void LoadDefaultSecondCatalogue()
        {
            LoadSecondCatalogue("DefaultFiles/sendrequests_default.txt");
        }
        public void LoadSecondCatalogue(string filePath)
        {
            var reader = new StreamReader(filePath);
            
            CatalogueIndex = 1;
            
            while (!reader.EndOfStream)
            {
                var data = reader.ReadLine()!.Split(';');
                Add(data);
            }
            
            CatalogueIndex = 0;
            
            reader.Close();
        }

        public void SaveCatalogue()
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = CurrentCatalogueName + "_Справочник",
                Filter = "Text files (*.txt)|*.txt",
            };

            if (saveDialog.ShowDialog() == true)
            {
                var writer = new StreamWriter(saveDialog.FileName);
                writer.Flush();
                
                if (CatalogueIndex == 0)
                {
                    foreach (var division in _divisionsData)
                    {
                        writer.WriteLine($"{division.Name};{division.Area};{division.Type}");
                    }
                }
                else
                {
                    foreach (var sendRequest in _sendRequestsData)
                    {
                        writer.WriteLine($"{sendRequest.DivisionName};{sendRequest.Client};{sendRequest.Service};{sendRequest.Date}");
                    }
                }
                
                writer.Close();
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
                return new SearchValuesDivisionsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:"));
            }

            return new SearchValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
        }

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


