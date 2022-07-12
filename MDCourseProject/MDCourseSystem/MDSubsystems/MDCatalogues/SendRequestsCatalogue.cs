using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class SendRequestsCatalogue:Catalogue
{
    public readonly LRBTree<DivisionNameAndArea, SendRequest> SendRequestsTree;
    private readonly List<SendRequest> _sendRequestsData;

    public readonly LRBTree<ClientFullNameAndTelephone, SendRequest> SendRequestsByClient;

    private static string FormatDate(string dateString)
    {
        var date = DateTime.Parse(dateString);
        return $"{date.Day:D2}.{date.Month:D2}.{date.Year:D4}";
    }
    
    public SendRequestsCatalogue()
    {
        SendRequestsTree = new LRBTree<DivisionNameAndArea, SendRequest>();
        _sendRequestsData = new List<SendRequest>();
        SendRequestsByClient = new LRBTree<ClientFullNameAndTelephone, SendRequest>();
    }
    
    public override void Add(string[] data)
    {
        data[4] = FormatDate(data[4]);
        
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequest(key, data[2], data[3], data[4]);
        
        SendRequestsTree.Add(key, value);

        var sendRequest = new SendRequest(key, data[2], data[3], data[4]);
        _sendRequestsData.Add(sendRequest);

        SendRequestsByClient.Add(
            key: UsefulMethods.GetClientFullNameAndTelephoneFromString(data[2]),
            val: sendRequest
        );
    }

    public override void Remove(string[] data)
    {
        data[4] = FormatDate(data[4]);
        
        var key = new DivisionNameAndArea(data[1], data[0]);
        var value = new SendRequest(key, data[2], data[3], data[4]);

        SendRequestsTree.Remove(key, value);
        
        var sendRequest = new SendRequest(key, data[2], data[3], data[4]);
        _sendRequestsData.RemoveAll(request => request.CompareTo(sendRequest)==0);
        
        SendRequestsByClient.Remove(
            key: UsefulMethods.GetClientFullNameAndTelephoneFromString(data[2]),
            val: sendRequest
        );
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var searchResult = new List<SendRequest>();
        var key = new DivisionNameAndArea(data[1], data[0]);
        if (SendRequestsTree.TryGetValuesList(key, out var list, out var steps))
        {
            searchResult.AddRange(list);
        }

        MDDebugConsole.Write($"Поиск в справочнике {Name} по ключу: <{data[0]}; {data[1]}> ", true);
        MDDebugConsole.WriteLine($"{ (steps>0 ? $"успешен! Найдено за: {steps} шагов" : $"неудачен! Значения не было найдено за: {steps} шагов")}", false);
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
            if (!savePath.EndsWith("SendRequest.txt"))
            {
                savePath = savePath.Replace(".txt", "_SendRequest.txt");
            }
            
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
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:", "Клиент:", "Название услуги:", "Дата:"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Район:", "Подразделение:"));
    }

    public override string PrintData()
    {
        return "\nСтатическая хэш-таблица по Подразделениям:\n"
               + MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsTable.ToStringWithStatuses() 
               + "\nЛевосторонее красно-черное дерево по Отправленным заявкам:\n" 
               + MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsTree.PrintTree()
               + "\n============================================================\n"
               + "\nЛевосторонее красно-черное дерево отправленных заявок Клиентов:\n" 
               + MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsByClient.PrintTree()
               + "\nЛевосторонее красно-черное дерево подразделений по названиям:\n"
               + MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsByName.PrintTree()
               + "\nЛевосторонее красно-черное дерево подразделений по районам:\n"
               + MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsByArea.PrintTree();
    }

    public override string Name => "Отправленные заявки";
}