using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class Client:IComparable<Client>
{
    public Client(string name, string surname, string patronymic, string telephone, string gender, DateTime date)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        Telephone = telephone;
        Gender = gender;
        Date = date;
    }

    public override string ToString()
    {
        return $"{Name} {Surname} {Patronymic} {Telephone} {Gender} {Date}";
    }

    public int CompareTo(Client other)
    {
        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        var surnameComparison = string.Compare(Surname, other.Surname, StringComparison.Ordinal);
        if (surnameComparison != 0) return surnameComparison;
        var patronymicComparison = string.Compare(Patronymic, other.Patronymic, StringComparison.Ordinal);
        if (patronymicComparison != 0) return patronymicComparison;
        var telephoneComparison = string.Compare(Telephone, other.Telephone, StringComparison.Ordinal);
        if (telephoneComparison != 0) return telephoneComparison;
        var genderComparison = string.Compare(Gender, other.Gender, StringComparison.Ordinal);
        if (genderComparison != 0) return genderComparison;
        return Date.CompareTo(other.Date);
    }
    
    public string Name {get; set;}
    public string Surname {get; set;}
    public string Patronymic {get; set;}
    public string Telephone {get; set;}
    public string Gender {get; set;}
    public DateTime Date {get; set;}
}

public class ClientFullNameAndTelephone:IComparable<ClientFullNameAndTelephone>
{
    public string Name {get; set;}
    public string Surname {get; set;}
    public string Patronymic {get; set;}
    public string Telephone {get; set;}

    public ClientFullNameAndTelephone(string name, string surname, string patronymic, string telephone)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        Telephone = telephone;
    }

    public int CompareTo(ClientFullNameAndTelephone other)
    {
        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        var surnameComparison = string.Compare(Surname, other.Surname, StringComparison.Ordinal);
        if (surnameComparison != 0) return surnameComparison;
        var patronymicComparison = string.Compare(Patronymic, other.Patronymic, StringComparison.Ordinal);
        if (patronymicComparison != 0) return patronymicComparison;
        return string.Compare(Telephone, other.Telephone, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() + Surname.GetHashCode() + Patronymic.GetHashCode() + Telephone.GetHashCode();
    }
    
    public override string ToString()
    {
        return Name + " " + Surname + " " + Patronymic + " (" + Telephone+ ")";
    }
}

public class Clients:Catalogue
{
    private DynamicHashTable<ClientFullNameAndTelephone, Client> _clientTable;
    private List<Client> CliestsInfo;
    public RB_Tree<int, Client> ClientAgeTree { get; }

    public Clients()
    {
        _clientTable = new DynamicHashTable<ClientFullNameAndTelephone, Client>();
        CliestsInfo = new List<Client>();
        ClientAgeTree = new RB_Tree<int, Client>();
        
        /*_clientTable.FirstHashFunc = key =>
        {
            return key % _clientTable.GetCapacity();
        };

        _clientTable.SecondHashFunc = key =>
        {
            return 1;
        };*/
        
        //TODO придумать хеш функции
    }

    public override void Add(string[] data)
    {
        var ClientInfo = new Client(data[0], data[1], data[2], data[3], data[4], DateTime.Parse(data[5]));
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);
        if (_clientTable.ContainsKey(key))
        {
            MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        _clientTable.Add(key,ClientInfo);
        CliestsInfo.Add(ClientInfo);
        ClientAgeTree.RBAddLeaf(DateTime.Today.Year - ClientInfo.Date.Year, ClientInfo);
    }

    public override void Remove(string[] data)
    {
        var ClientInfo = new Client(data[0], data[1], data[2], data[3], data[4], DateTime.Parse(data[5]));
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);
        _clientTable.Remove(key,ClientInfo);
        CliestsInfo.RemoveAll(client => client.CompareTo(ClientInfo) == 0);
        ClientAgeTree.RBDelete(DateTime.Today.Year - ClientInfo.Date.Year, ClientInfo);
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);

        if ( _clientTable.TryGetValue(key, out var res))
            PrintDataToGrid(mainDataGrid, new List<Client> {res}, new[] {"Имя", "Фамилия", "Отчество", "Телефон", "Пол", "Дата рождения"});
        else
            MessageBox.Show("Элемент не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);

    }

    public override void PrintDataToGrid(DataGrid mainDataGrid)
    {
        PrintDataToGrid(mainDataGrid, CliestsInfo, new []{"Имя", "Фамилия", "Отчество", "Телефон", "Пол", "Дата рождения"});
    }

    public override void Load(string filePath)
    {
        var input = new StreamReader(filePath);
        while (!input.EndOfStream)
            Add(input.ReadLine()?.Split('|'));
        input.Close();
    }

    public override void Save()
    {
        if (OpenSaveCatalogueDialog(Name, out var filePath))
        {
            var output = new StreamWriter(filePath);
            output.Flush();
                
            foreach(var client in CliestsInfo)
                output.WriteLine(string.Join("|", client.Name, client.Surname, client.Patronymic, client.Telephone, client.Gender, client.Date));
            output.Close();
        }
    }

    public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
    {
        return new AddValuesClientsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя", "Фамилия", "Отчество", "Телефон", "Пол", "Дата рождения"));
    }

    public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя", "Фамилия", "Отчество", "Телефон", "Пол", "Дата рождения"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя", "Фамилия", "Отчество", "Телефон"));
    }

    public override string PrintData()
    {
        return String.Empty;
    }

    public override string Name => "Клиенты";

    public DynamicHashTable<ClientFullNameAndTelephone, Client> ClientsTable => _clientTable;
}