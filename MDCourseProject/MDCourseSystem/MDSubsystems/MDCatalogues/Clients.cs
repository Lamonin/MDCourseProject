using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class Client:IComparable<Client>
{
    public string Name {get; set;}
    public string Surname {get; set;}
    public string Patronymic {get; set;}
    public string Telephone {get; set;}
    public string Gender {get; set;}
    //public DateTime Date {get; set;}
    public string Date {get; set;}

    public Client(string name, string surname, string patronymic, string telephone, string gender, string date)
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
        if (Telephone == other.Telephone) return 0;
        else return 1;
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
        _clientTable.FirstHashFunc = key =>
        {
            var intValue = Math.Abs(key.GetHashCode());
            string stroke = key.ToString();
            foreach (var i in stroke)
            {
                intValue +=  i;
            }
            return (uint)(intValue % _clientTable.GetCapacity());
        };

        _clientTable.SecondHashFunc = key =>
        {
            var intValue = Math.Abs(key.GetHashCode());
            string stroke = key.ToString();
            foreach (var i in stroke)
            {
                intValue += (int) i;
            }

            intValue %= 13;
            return (uint)(intValue % _clientTable.GetCapacity() + 1);
        };
    }

    public override void Add(string[] data)
    {
        var ClientInfo = new Client(data[0], data[1], data[2], data[3], data[4], data[5]);
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);
        if (_clientTable.ContainsKey(key))
        {
            MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        _clientTable.Add(key,ClientInfo);
        CliestsInfo.Add(ClientInfo);
        ClientAgeTree.RBAddLeaf(DateTime.Today.Year - int.Parse(data[5].Split('.')[2]),ClientInfo);
    }

    public override void Remove(string[] data)
    {
        var ClientInfo = new Client(data[0], data[1], data[2], data[3], data[4], data[5]);
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);
        _clientTable.Remove(key,ClientInfo);
        CliestsInfo.RemoveAll(Client => Client.CompareTo(ClientInfo) == 0);
        ClientAgeTree.RBDelete(DateTime.Today.Year - int.Parse(data[5].Split('.')[2]), ClientInfo);

        var client = (data[0] + " " + data[1] + " " + data[2] + " " + data[3]).Split();
        MDSystem.clientsSubsystem._applications.RemoveByClient(client);
        var keyForDelInDivisionsSubsystem = new ClientFullNameAndTelephone(data[1],data[0],data[2],data[3]);
        MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsByClient.TryGetValuesList(keyForDelInDivisionsSubsystem, out var list);
        foreach (var i in list)
        {
            string[] remove = new string[5];
            remove[0] = i.Division.Area;
            remove[1] = i.Division.Name;
            remove[2] = i.Client;
            remove[3] = i.Service;
            remove[4] = i.Date;
            MDSystem.divisionsSubsystem.SendRequestsCatalogue.Remove(remove);
        }
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var key = new ClientFullNameAndTelephone(data[0],data[1],data[2],data[3]);

        if (_clientTable.TryGetValue(key, out var res, out var steps))
        {
            PrintDataToGrid(mainDataGrid, new List<Client> {res}, new[] {"Имя", "Фамилия", "Отчество", "Телефон", "Пол", "Дата рождения"});
            MDDebugConsole.WriteLine($"Клиент по ключу <{key.Name} {key.Surname} {key.Patronymic} {key.Telephone}> был успешно найден за {steps} шагов!",true);
        }
        else
        {
          MessageBox.Show("Элемент не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
          MDDebugConsole.WriteLine($"Клиент по ключу <{key.Name} {key.Surname} {key.Patronymic} {key.Telephone}> не существует в таблице!",true);
        }
        
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
        return "Хеш-таблица:\n \n" + _clientTable.ToStringWithStatuses()
                                   + "\nДерево (\"Клиенты\" - \"Клиенты\" по возрасту клиентов):\n \n" +
                                   ClientAgeTree.PrintTree();
        //  сделать отладку
    }

    public override string Name => "Клиенты";

    public DynamicHashTable<ClientFullNameAndTelephone, Client> ClientsTable => _clientTable;
}