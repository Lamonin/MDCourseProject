using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public struct SendRequest:IComparable<SendRequest>
{
    public readonly DivisionNameAndArea Division;
    public SendRequest(DivisionNameAndArea division, string client, string service, string date)
    {
        Division = division;
        DivisionName = Division.ToString();
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
    public readonly LRBTree<DivisionNameAndArea, SendRequest> SendRequestsTree;
    private readonly List<SendRequest> _sendRequestsData;

    public readonly LRBTree<string, SendRequest> SendRequestsByService;

    public SendRequestsCatalogue()
    {
        SendRequestsTree = new LRBTree<DivisionNameAndArea, SendRequest>();
        _sendRequestsData = new List<SendRequest>();

        SendRequestsByService = new LRBTree<string, SendRequest>();
    }
    
    public override void Add(string[] data)
    {
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequest(key, data[2], data[3], data[4]);
                
        SendRequestsTree.Add(key, value);
        MDDebugConsole.WriteLine(key.ToString());

        var sendRequest = new SendRequest(key, data[2], data[3], data[4]);
        _sendRequestsData.Add(sendRequest);
        SendRequestsByService.Add(data[3], sendRequest);
    }

    public override void Remove(string[] data)
    {
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequest(key, data[2], data[3], data[4]);

        SendRequestsTree.Remove(key, value);
        
        var sendRequest = new SendRequest(key, data[2], data[3], data[4]);
        _sendRequestsData.RemoveAll(request => request.CompareTo(sendRequest)==0);
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var searchResult = new List<SendRequest>();
        var key = new DivisionNameAndArea(data[1], data[0]);
        if (SendRequestsTree.TryGetValuesList(key, out var list)) { searchResult.AddRange(list); }
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
        return new SearchValuesSendRequestsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:"));
    }

    public override string Name => "Отправленные заявки";
}