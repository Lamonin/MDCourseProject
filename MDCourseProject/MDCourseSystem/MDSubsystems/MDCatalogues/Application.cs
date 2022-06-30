using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class Staff:IComparable<Staff>
{
    public string StaffName;
    
    public string StaffSurname;
    
    public string StaffPatronymic;

    public string StaffOccupation;
    
    public int CompareTo(Staff other)
    {
        var staffNameComparison = string.Compare(StaffName, other.StaffName, StringComparison.Ordinal);
        if (staffNameComparison != 0) return staffNameComparison;
        var staffSurnameComparison = string.Compare(StaffSurname, other.StaffSurname, StringComparison.Ordinal);
        if (staffSurnameComparison != 0) return staffSurnameComparison;
        var staffPatronymicComparison = string.Compare(StaffPatronymic, other.StaffPatronymic, StringComparison.Ordinal);
        if (staffPatronymicComparison != 0) return staffPatronymicComparison;
        return string.Compare(StaffOccupation, other.StaffOccupation, StringComparison.Ordinal);
    }

    public Staff(string staffName, string staffSurname, string staffPatronymic, string staffOccupation)
    {
        StaffName = staffName;
        StaffSurname = staffSurname;
        StaffPatronymic = staffPatronymic;
        StaffOccupation = staffOccupation;
    }
}
public class Application : IComparable<Application>
{
    public Staff staff;

    public string ClientName;
    
    public string ClientSurname;
    
    public string ClientPatronymic;

    public string ClientTelephone;
    
    public DateTime Date;
    
    public override string ToString()
    {
        return $"{staff} {ClientName} {ClientSurname} {ClientPatronymic} {ClientTelephone} {Date}";
    }

    public Application(Staff staff, string clientName, string clientSurname, string clientPatronymic, string clientTelephone, DateTime date)
    {
        this.staff = staff;
        ClientName = clientName;
        ClientSurname = clientSurname;
        ClientPatronymic = clientPatronymic;
        ClientTelephone = clientTelephone;
        Date = date;
    }

    public int CompareTo(Application other)
    {
        var staffComparison = Comparer<Staff>.Default.Compare(staff, other.staff);
        if (staffComparison != 0) return staffComparison;
        var clientNameComparison = string.Compare(ClientName, other.ClientName, StringComparison.Ordinal);
        if (clientNameComparison != 0) return clientNameComparison;
        var clientSurnameComparison = string.Compare(ClientSurname, other.ClientSurname, StringComparison.Ordinal);
        if (clientSurnameComparison != 0) return clientSurnameComparison;
        var clientPatronymicComparison = string.Compare(ClientPatronymic, other.ClientPatronymic, StringComparison.Ordinal);
        if (clientPatronymicComparison != 0) return clientPatronymicComparison;
        var clientTelephoneComparison = string.Compare(ClientTelephone, other.ClientTelephone, StringComparison.Ordinal);
        if (clientTelephoneComparison != 0) return clientTelephoneComparison;
        return Date.CompareTo(other.Date);
    }
}

public class Applications:Catalogue
{
    private RB_Tree<Staff, Application> tree;
    private List<Application> ApplicationsInfo;

    public Applications()
    {
        tree = new RB_Tree<Staff, Application>();
        ApplicationsInfo = new List<Application>();
    }

    public override void Add(string[] data)
    {
        var key = new Staff(data[0],data[1],data[2],data[3]);
        var ApplicationInfo = new Application(key, data[4], data[5], data[6], data[7], DateTime.Parse(data[7]));
        tree.RBAddLeaf(key,ApplicationInfo);
        ApplicationsInfo.Add(ApplicationInfo);
    }

    public override void Remove(string[] data)
    {
        var key = new Staff(data[0],data[1],data[2],data[3]);
        var ApplicationInfo = new Application(key, data[4], data[5], data[6], data[7], DateTime.Parse(data[7]));
        tree.RBDelete(key,ApplicationInfo);
        ApplicationsInfo.Remove(ApplicationInfo);
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var key = new Staff(data[0],data[1],data[2],data[3]);
        var res = tree.GetLeaf(tree.m_root, key).valList.head.pData;
        if(res != null)
            PrintDataToGrid(mainDataGrid, new List<Application> {res}, new[] {"Имя сотрудника","Фамилия сотрудника","Отчество сотрудника",
                "Должность сотрудника","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"});
        else
            MessageBox.Show("Элемент не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public override void PrintDataToGrid(DataGrid mainDataGrid)
    {
        PrintDataToGrid(mainDataGrid, ApplicationsInfo, new []{"Имя сотрудника","Фамилия сотрудника","Отчество сотрудника",
            "Должность сотрудника","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"});
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
                
            foreach(var application in ApplicationsInfo)
                output.WriteLine(string.Join("|", application.staff,application.ClientName,application.ClientSurname,application.ClientPatronymic,application.ClientTelephone,application.Date));
            output.Close();
        }
    }

    public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
    {
        return new AddValuesApplicationAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя сотрудника","Фамилия сотрудника","Отчество сотрудника",
            "Должность сотрудника","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"));
    }

    public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя сотрудника","Фамилия сотрудника","Отчество сотрудника",
            "Должность сотрудника","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"));
    }

    public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя сотрудника","Фамилия сотрудника","Отчество сотрудника",
            "Должность сотрудника","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"));
    }

    public override string PrintData()
    {
        return String.Empty;
    }

    public override string Name { get; }

    public RB_Tree<Staff, Application> _tree => tree;
}