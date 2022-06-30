using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

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

    public override int GetHashCode()
    {
        return Name.GetHashCode() + Area.GetHashCode();
    }

    public override string ToString()
    {
        return Name + " (" + Area + ")";
    }

    public readonly string Name;
    public readonly string Area;
}

public class DivisionsCatalogue:Catalogue
{
    public readonly StaticHashTable<DivisionNameAndArea, string> DivisionsTable;
    private readonly List<Division> _divisionsData;

    public readonly LRBTree<string, Division> DivisionsByName;
    public readonly LRBTree<string, Division> DivisionsByArea;

    public DivisionsCatalogue()
    {
        DivisionsTable = new StaticHashTable<DivisionNameAndArea, string>(1000);
        _divisionsData = new List<Division>();
        
        DivisionsByName = new LRBTree<string, Division>();
        DivisionsByArea = new LRBTree<string, Division>();
    }

    public override void Add(string[] data)
    {
        var key = new DivisionNameAndArea(data[0], data[1]);
        
        MDDebugConsole.WriteLine($"Добавление в таблицу по ключу: {key.ToString()}; Первичная ХФ: {DivisionsTable.FirstHashFunc(key.GetHashCode())}; Вторичная ХФ: {DivisionsTable.SecondHashFunc(key.GetHashCode())}");
        DivisionsTable.Add(key, data[2]);

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
        }
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        MDDebugConsole.WriteLine($"Поиск в справочнике {Name} значения: {data[0]};{data[1]}");
        
        var searchResult = new List<Division>();
        if (DivisionsTable.TryGetValue(new DivisionNameAndArea(data[0], data[1]), out var value))
        {
            searchResult.Add(new Division(data[0], data[1], value));
        }
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
        return "Статическая хэш-таблица по Подразделениям:\n" + DivisionsTable.ToStringWithStatuses();
    }

    public override string Name => "Подразделения";
}