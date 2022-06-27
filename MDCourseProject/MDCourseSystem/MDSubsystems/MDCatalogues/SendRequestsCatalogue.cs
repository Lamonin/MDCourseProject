using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

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

public class SendRequestsCatalogue:Catalogue
{
    private LRBTree<DivisionNameAndArea, SendRequestClientServiceAndDate> _sendRequestTree;
    private ObservableCollection<SendRequest> _sendRequestsData;

    public SendRequestsCatalogue()
    {
        _sendRequestTree = new LRBTree<DivisionNameAndArea, SendRequestClientServiceAndDate>();
        _sendRequestsData = new ObservableCollection<SendRequest>();
    }
    
    public override void Add(string[] data)
    {
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequestClientServiceAndDate(data[2], data[3], data[4]);
                
        _sendRequestTree.Add(key, value);
        _sendRequestsData.Add(new SendRequest(key.ToString(), data[2], data[3], data[4]));
    }

    public override void Remove(string[] data)
    {
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequestClientServiceAndDate(data[2], data[3], data[4]);

        _sendRequestTree.Remove(key, value);
        _sendRequestsData.Remove(new SendRequest(key.ToString(), data[2], data[3], data[4]));
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var searchResult = new ObservableCollection<SendRequest>();
        //TODO Дереву надо метод TryGetValue добавить
        PrintDataToGrid(mainDataGrid, searchResult, new []{"Подразделение", "Клиент", "Услуга", "Дата"});
    }

    public override void PrintDataToGrid(DataGrid mainDataGrid)
    {
        PrintDataToGrid(mainDataGrid, _sendRequestsData, new []{"Подразделение", "Клиент", "Услуга", "Дата"});
    }

    public override void Load(string filePath)
    {
        var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var data = reader.ReadLine()!.Split(';');
            Add(data);
        }
            
        reader.Close();
    }

    public override void Save()
    {
        if (OpenSaveCatalogueDialog(Name, out var savePath))
        {
            var writer = new StreamWriter(savePath);
            writer.Flush();
            
            foreach (var sendRequest in _sendRequestsData)
                writer.WriteLine($"{sendRequest.DivisionName};{sendRequest.Client};{sendRequest.Service};{sendRequest.Date}");
            
            writer.Close();
        }
    }

    public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
    {
        return new AddValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
    }

    public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
    {
        return new RemoveValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new SearchValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
    }

    public override string Name => "Отправленные заявки";
}