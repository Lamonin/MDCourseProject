using System;
using System.Collections.Generic;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDSubsystems
{
    public readonly struct Division:IComparable<Division>
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
        
        public readonly string Name;
        public readonly string Area;
        public readonly string Type;
    }
    
    public readonly struct SendRequest:IComparable<SendRequest>
    {
        public SendRequest(Division division, string client, string service, string date)
        {
            Division = division;
            Client = client;
            Service = service;
            Date = date;
        }

        public int CompareTo(SendRequest other)
        {
            var compareRes = Division.CompareTo(other.Division);
            if (compareRes != 0) return compareRes; 
            
            compareRes = string.Compare(Client, other.Client, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            compareRes = string.Compare(Service, other.Service, StringComparison.OrdinalIgnoreCase);
            if (compareRes != 0) return compareRes;
            
            return string.Compare(Date, other.Date, StringComparison.OrdinalIgnoreCase);
        }
        
        public readonly Division Division;
        public readonly string Client;
        public readonly string Service;
        public readonly string Date;
    }
    
    public class DivisionsSubsystem:ISubsystem
    {
        private int _catalogueIndex;
        
        public DivisionsSubsystem()
        {
            DivisionsTable = new StaticHashTable<string, Division>(1000);
            SendRequestsTree = new LRBTree<string, SendRequest>();
            CatalogueIndex = 0;
        }
        
        public StaticHashTable<string, Division> DivisionsTable { get; private set; }
        public LRBTree<string, SendRequest> SendRequestsTree { get; private set; }

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


