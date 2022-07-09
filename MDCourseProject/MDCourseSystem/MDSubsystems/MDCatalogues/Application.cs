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
        if (other == null) return 1;
        var staffNameComparison = string.Compare(StaffName, other.StaffName, StringComparison.Ordinal);
        if (staffNameComparison != 0) return staffNameComparison;
        var staffSurnameComparison = string.Compare(StaffSurname, other.StaffSurname, StringComparison.Ordinal);
        if (staffSurnameComparison != 0) return staffSurnameComparison;
        var staffPatronymicComparison = string.Compare(StaffPatronymic, other.StaffPatronymic, StringComparison.Ordinal);
        if (staffPatronymicComparison != 0) return staffPatronymicComparison;
        return string.Compare(StaffOccupation, other.StaffOccupation, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return String.Join(" ", StaffName, StaffSurname, StaffPatronymic, StaffOccupation);
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
    public Staff staff { get; set; }

    public string ClientName  { get; set; }
    
    public string ClientSurname  { get; set; }
    
    public string ClientPatronymic  { get; set; }

    public string ClientTelephone  { get; set; }
    
    //public DateTime Date { get; set; }
    public string Date { get; set; }
    
    public override string ToString()
    {
        return $"{staff} {ClientName} {ClientSurname} {ClientPatronymic} {ClientTelephone} {Date}";
    }

    public Application(Staff staff, string clientName, string clientSurname, string clientPatronymic, string clientTelephone, string date)
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
        var ApplicationInfo = new Application(key, data[4], data[5], data[6], data[7], data[8]);
        tree.RBAddLeaf(key,ApplicationInfo);
        ApplicationsInfo.Add(ApplicationInfo);
    }

    public override void Remove(string[] data)
    {
        var key = new Staff(data[0],data[1],data[2],data[3]);
        var ApplicationInfo = new Application(key, data[4], data[5], data[6], data[7], data[8]);
        tree.RBDelete(key,ApplicationInfo);
        ApplicationsInfo.RemoveAll(application => application.CompareTo(ApplicationInfo) == 0);
    }
    
    public void RemoveByClient(string[] data)
    {
        bool deleted = true;
        while (deleted)
        {
           foreach(var i in ApplicationsInfo)
           {
               if (i.ClientName == data[0] && i.ClientSurname == data[1] && i.ClientPatronymic == data[2] &&
                   i.ClientTelephone == data[3])
               {
                   string[] remove = new string[9];
                   remove[0] = i.staff.StaffName;
                   remove[1] = i.staff.StaffSurname;
                   remove[2] = i.staff.StaffPatronymic;
                   remove[3] = i.staff.StaffOccupation;
                   remove[4] = i.ClientName;
                   remove[5] = i.ClientSurname;
                   remove[6] = i.ClientPatronymic;
                   remove[7] = i.ClientTelephone;
                   remove[8] = i.Date;
                   Remove(remove);
                   deleted = true;
                   break;
               }
               else deleted = false;
           } 
        }
        
    }

    public override void Find(DataGrid mainDataGrid, string[] data)
    {
        var key = new Staff(data[0],data[1],data[2],data[3]);
        var head = tree.GetLeaf(tree.m_root, key).valList.head;
        if (head != null)
        {
            var results = new List<Application>();
            var node = head;
            do
            {
                results.Add(node.pData);
                node = node.pNext;
            } while (node != null);
                
            PrintDataToGrid(mainDataGrid, results, new[] {"Сотрудник","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"});
        }
        else
            MessageBox.Show("Элемент не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public override void PrintDataToGrid(DataGrid mainDataGrid)
    {
        PrintDataToGrid(mainDataGrid, ApplicationsInfo, new []{"Сотрудник","Имя клиента","Фамилия клиента", "Отчество клиента","Телефон клиента", "Дата"});
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
        return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Имя сотрудника","Фамилия сотрудника","Отчество сотрудника", "Должность сотрудника"));
    }

    public override string PrintData()
    {
        return String.Empty;
    }

    public override string Name { get; }

    public RB_Tree<Staff, Application> _tree => tree;
}