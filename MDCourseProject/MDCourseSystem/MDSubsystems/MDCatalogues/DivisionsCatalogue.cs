using System;
using System.IO;
using System.Collections.ObjectModel;
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
    private readonly ObservableCollection<Division> _divisionsData;
    
    public DivisionsCatalogue()
    {
        DivisionsTable = new StaticHashTable<DivisionNameAndArea, string>(1000);
        _divisionsData = new ObservableCollection<Division>();
    }

    public override void Add(string[] data)
    {
        var key = new DivisionNameAndArea(data[0], data[1]);
        
        MDDebugConsole.WriteLine($"Добавление в таблицу по ключу: {key.ToString()}; Первчиная ХФ: {DivisionsTable.FirstHashFunc(key.GetHashCode())}; Вторичная ХФ: {DivisionsTable.SecondHashFunc(key.GetHashCode())}");
        DivisionsTable.Add(key, data[2]);
        _divisionsData.Add(new Division(data[0], data[1], data[2]));
    }

    public override void Remove(string[] data)
    {
        var key = new DivisionNameAndArea(data[0], data[1]);
        
        if (DivisionsTable.TryGetValue(key, out var value))
        {
            DivisionsTable.Remove(key, value);
            _divisionsData.Remove(new Division(key.Name, key.Area, value));
        }
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        MDDebugConsole.WriteLine($"Поиск в справочнике {Name} значения: {data[0]};{data[1]}");
        
        var searchResult = new ObservableCollection<Division>();
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
        return new RemoveValuesDivisionsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Подразделение:", "Район:"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new SearchValuesDivisionsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Название подразделения:", "Район:"));
    }

    public override string Name => "Подразделения";
}