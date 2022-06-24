using System;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;

namespace MDCourseProject.MDCourseSystem.MDCatalogues.Divisions
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
    
    public class DivisionsCatalogueHandler
    {
        public DivisionsCatalogueHandler()
        {
            DivisionsTable = new StaticHashTable<string, Division>(1000);
            SendRequestsTree = new LRBTree<string, SendRequest>();
        }

        public DataAnalyser GetAnalyser()
        {
            return null;
        }
        
        public string[] CatalogueNames => new[]{"Подразделения", "Отправленные заявки"};
        public StaticHashTable<string, Division> DivisionsTable { get; private set; }
        public LRBTree<string, SendRequest> SendRequestsTree { get; private set; }
    }
}


