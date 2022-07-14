using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FundamentalStructures;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using MDCourseProject.MDCourseSystem.MDCatalogues;
using Microsoft.Win32;
using Application = MDCourseProject.MDCourseSystem.MDCatalogues.Application;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class ClientsSubsystem:ISubsystem
{
    public Applications _applications { get;}
    public Clients _clients { get;}

    public ClientsSubsystem()
    {
        _applications = new Applications();
        _clients = new Clients();
    }
    

    public void LoadDefaultFirstCatalogue()
    {
        
        LoadFirstCatalogue("DefaultFiles/default_clients.txt");
    }

    public void LoadDefaultSecondCatalogue()
    {
        LoadSecondCatalogue("DefaultFiles/default_applications.txt");
    }
    
    public void LoadFirstCatalogue(string filePath)
    {
        if (!filePath.EndsWith("clients.txt"))
        {
            MessageBox.Show("Некорректный файл для справочника Клиенты!", "Ошибка!", MessageBoxButton.OK);
            throw new Exception("Incorrect file!");
        }
        _clients.Load(filePath);
    }

    public void LoadSecondCatalogue(string filePath)
    {
        if (!filePath.EndsWith("applications.txt"))
        {
            MessageBox.Show("Некорректный файл для справочника Обращения!", "Ошибка!", MessageBoxButton.OK);
            throw new Exception("Incorrect file!");
        }
        _applications.Load(filePath);
    }

    public bool MakeReport(string[] data)
    {
        var saveReportDialog = new SaveFileDialog
        {
            Title = "Выберите место для сохранения отчета \"Обращения клиента\"",
            FileName ="Отчет \"Обращения клиента\"",
            Filter = "Text files (*.txt)|*.txt",
        };
        
        //data[0] = фио сотрудника
        //data[1] = должность сотрудника
        //dara[2] = возраст клиента
        if (saveReportDialog.ShowDialog() == true)
        {  
            //итоговый список
            var reportResults = new List<Application>();

            var StaffFIO = data[0].Split();
            var StaffKey = new Staff(StaffFIO[0], StaffFIO[1], StaffFIO[2], data[1]);
            
            //список обращений к сотруднику по фио и должности
            var ApplicationsTreeListByStaff = new RB_Tree<Staff, Application>.leaf.list();

            if (_applications._tree.GetLeaf(_applications._tree.m_root, StaffKey) != null)
            {
                ApplicationsTreeListByStaff =
                        _applications._tree.GetLeaf(_applications._tree.m_root, StaffKey).valList; // поломка тут
    
                var ApplicationsListByStaff = new List<Application>();
                
                //заполняю список обращений к сотруднику по фио и должности
                var curr = ApplicationsTreeListByStaff.head;
                while (curr != null)
                {
                    ApplicationsListByStaff.Add(curr.pData);
                    curr = curr.pNext;
                }
                
                //заполняю итоговый список
                foreach (var application in ApplicationsListByStaff)
                {
                    var clientKey = new ClientFullNameAndTelephone(application.ClientName, application.ClientSurname,
                        application.ClientPatronymic, application.ClientTelephone);
    
                    _clients.ClientsTable.TryGetValue(clientKey, out var client);
                    
                    int ClientYear = int.Parse(client.Date.Split('.')[2]);
    
                    if ((DateTime.Today.Year - ClientYear) == int.Parse(data[2]))
                    {
                        reportResults.Add(application);
                    }
                }
            }
            
            var writer = new StreamWriter(saveReportDialog.FileName);
            
            if (reportResults.Count == 0)
            {
                writer.WriteLine("Не было найдено ни одной записи удовлетворяющей условиям!");
            }

            else
            {
                foreach (var result in reportResults)
                    writer.WriteLine(result.ToString());
            }
            
            writer.Close();
            
            Process.Start(saveReportDialog.FileName);

            return true;

        }
        
        return false;
    }

    public DataAnalyser BuildReportWindow(Grid mainGrid)
    {
        mainGrid.RowDefinitions.Clear();
        CommonWindowGenerator.GenerateRowsInGrid(mainGrid, 3);
        
        var tList = new TextBox[]
        {
            CommonWindowGenerator.CreateInputField(mainGrid, "ФИО сотрудника:", 0),
            CommonWindowGenerator.CreateInputField(mainGrid, "Должность сотрудника:", 2),
            CommonWindowGenerator.CreateInputField(mainGrid, "Возраст клиента:", 4),
        };
        return new ReportClientsAnalyser(tList);
        //return null;
    }

    private int _catalogueIndex;
    public int CatalogueIndex
    {
        get => _catalogueIndex;
        set
        {
            if (value < 0) value = 0;
            _catalogueIndex = value;
        }
    }

    public Catalogue Catalogue => _catalogueIndex == 0 ? _clients : _applications;

    public IEnumerable<string> CataloguesNames => new []{"Клиенты", "Обращения"};
}