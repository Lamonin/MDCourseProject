using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class DivisionsCatalogue:Catalogue
{
    public readonly StaticHashTable<DivisionNameAndArea, string> DivisionsTable;
    private readonly List<Division> _divisionsData;

    public readonly LRBTree<string, Division> DivisionsByName;
    public readonly LRBTree<string, Division> DivisionsByArea;

    public DivisionsCatalogue()
    {
        DivisionsTable = new StaticHashTable<DivisionNameAndArea, string>(997);
        _divisionsData = new List<Division>();
        
        DivisionsByName = new LRBTree<string, Division>();
        DivisionsByArea = new LRBTree<string, Division>();
    }

    public override void Add(string[] data)
    {
        var key = new DivisionNameAndArea(data[0], data[1]);
        try
        {
            DivisionsTable.Add(key, data[2]);
        }
        catch
        {
            MessageBox.Show("В хеш-таблице Подразделения закончилось место!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var division = new Division(data[0], data[1], data[2]);
        DivisionsByName.Add(data[0], division);
        DivisionsByArea.Add(data[1], division);
        _divisionsData.Add(division);
    }

    public override void Remove(string[] data)
    {
        var key = new DivisionNameAndArea(data[0], data[1]);
        
        if (DivisionsTable.TryGetValue(key, out var value))
        {
            DivisionsTable.Remove(key, value);
            
            var division = new Division(key.Name, key.Area, value);
            _divisionsData.Remove(division);
            DivisionsByName.Remove(key.Name, division);
            DivisionsByArea.Remove(key.Area, division);
            
            //Удаляем все отправленные заявки этого подразделения
            if (MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsTree.TryGetValuesList(key, out var list))
            {
                foreach (var s in list)
                {
                    MDSystem.divisionsSubsystem.SendRequestsCatalogue.Remove(new []{key.Area, key.Name, s.Client, s.Service, s.Date});
                }
            }

            if (DivisionsByName.Contains(data[0])) return;
            // Если подразделение было удалено окончательно,
            // то удаляем связанные с ним документы в подсистеме Staff
            
            var divisionName = new DivisionName(data[0]);
            if (MDSystem.staffSubsystem.DocumentCatalogue.DivisionName.ContainKey(divisionName))
            {
                var documentInfos = MDSystem.staffSubsystem.DocumentCatalogue.DivisionName.GetValue(divisionName);
                foreach (var document in documentInfos)
                {
                    MDSystem.staffSubsystem.DocumentCatalogue.Remove(new []
                    {
                        document.Document.ToString(),
                        document.Occupation.ToString(),
                        document.DivisionName.ToString()
                    });
                }
            }
        }
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var searchResult = new List<Division>();
        if (DivisionsTable.TryGetValue(new DivisionNameAndArea(data[0], data[1]), out var value, out var steps))
        {
            searchResult.Add(new Division(data[0], data[1], value));
        }
        
        MDDebugConsole.Write($"Поиск в справочнике {Name} по ключу: <{data[0]}; {data[1]}> ", true);
        MDDebugConsole.WriteLine($"{ (steps>0 ? $"успешен! Найдено за: {steps} шагов" : $"неудачен! Значения не было найдено за: {steps} шагов")}", false);
        PrintDataToGrid(mainDataGrid, searchResult, new []{"Подразделение", "Район", "Тип подразделения"});
    }

    public override void PrintDataToGrid(DataGrid mainDataGrid)
    {
       PrintDataToGrid(mainDataGrid, _divisionsData, new []{"Подразделение", "Район", "Тип подразделения"});
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
            if (!savePath.EndsWith("Division.txt"))
            {
                savePath = savePath.Replace(".txt", "_Division.txt");
            }

            var writer = new StreamWriter(savePath);
            writer.Flush();
            
            foreach (var division in _divisionsData)
                writer.WriteLine($"{division.Name};{division.Area};{division.Type}");
            
            writer.Close();
        }
    }

    public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
    {
        return new AddValuesDivisionAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:", "Тип подразделения:"));
    }

    public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Район:"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:"));
    }

    public override string PrintData()
    {
        Console.Out.WriteLine(MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsTable.ToStringWithStatuses());
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

    public override string Name => "Подразделения";
}