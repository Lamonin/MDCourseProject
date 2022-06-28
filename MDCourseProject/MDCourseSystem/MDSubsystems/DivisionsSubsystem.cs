using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using MDCourseProject.MDCourseSystem.MDCatalogues;
using Microsoft.Win32;

namespace MDCourseProject.MDCourseSystem.MDSubsystems
{
    public class DivisionsSubsystem:ISubsystem
    {
        private int _catalogueIndex;
        private DivisionsCatalogue _divisionsCatalogue;
        private SendRequestsCatalogue _sendRequestsCatalogue;
        
        public DivisionsSubsystem()
        {
            CatalogueIndex = 0;
            _divisionsCatalogue = new DivisionsCatalogue();
            _sendRequestsCatalogue = new SendRequestsCatalogue();
        }

        public void LoadDefaultFirstCatalogue()
        {
            LoadFirstCatalogue("DefaultFiles/divisions_default.txt");
        }
        public void LoadFirstCatalogue(string filePath)
        {
            _divisionsCatalogue.Load(filePath);
        }

        public void LoadDefaultSecondCatalogue()
        {
            LoadSecondCatalogue("DefaultFiles/sendrequests_default.txt");
        }
        public void LoadSecondCatalogue(string filePath)
        {
            _sendRequestsCatalogue.Load(filePath);
        }

        public bool MakeReport(string[] data)
        {
            var saveReportDialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения отчета по подсистеме Подразделения",
                FileName ="Подразделения_Отчет",
                Filter = "Text files (*.txt)|*.txt",
            };
        
            if (saveReportDialog.ShowDialog() == true)
            {
                if (SendRequestsCatalogue.SendRequestsByService.TryGetValuesList(data[0], out var list))
                {
                    var reportResults = new List<SendRequest>();

                    foreach (var send in list)
                    {
                        if (DateTime.Parse(send.Date).CompareTo(DateTime.Parse(data[1]))>=0 && DateTime.Parse(send.Date).CompareTo(DateTime.Parse(data[2]))<=0)
                        {
                            if (String.Compare(send.Division.Area, data[3], StringComparison.OrdinalIgnoreCase)==0)
                            {
                                reportResults.Add(send);
                            }
                        }
                    }
                    
                    var writer = new StreamWriter(saveReportDialog.FileName);
                    foreach (var result in reportResults)
                        writer.WriteLine(result.ToString());
                    writer.Close();
                    
                    Process.Start(saveReportDialog.FileName);
                }
                else
                {
                    Console.Out.WriteLine("Анализатор не справился!");
                    return false;
                }

                return true;
            }
            
            return false;
        }

        public DataAnalyser BuildReportWindow(Grid mainGrid)
        {
            mainGrid.RowDefinitions.Clear();
        
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)});
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)});
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(28)});
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(8)});

            var tBoxes = CommonWindowGenerator.CreateReportInputBetweenField(mainGrid, "Дата:", 2);
            var tList = new TextBox[]
            {
                CommonWindowGenerator.CreateInputField(mainGrid, "Услуга:", 0),
                tBoxes[0],
                tBoxes[1],
                CommonWindowGenerator.CreateInputField(mainGrid, "Район:", 4)
            };
            return new ReportDivisionsAnalyser(tList);
        }

        public DivisionsCatalogue DivisionsCatalogue => _divisionsCatalogue;
        public SendRequestsCatalogue SendRequestsCatalogue => _sendRequestsCatalogue;

        public int CatalogueIndex
        {
            get => _catalogueIndex;
            set
            {
                if (value < 0) value = 0;
                _catalogueIndex = value;

                //Этот вывод в консоль так... для красоты)
                if (_catalogueIndex == 0)
                    MDDebugConsole.WriteLine("Выбран справочник Подразделения");
                else
                    MDDebugConsole.WriteLine("Выбран справочник Отправленные заявки");
            }
        }

        public Catalogue Catalogue => _catalogueIndex == 0 ? _divisionsCatalogue : _sendRequestsCatalogue;

        public IEnumerable<string> CataloguesNames => new[]{_divisionsCatalogue.Name, _sendRequestsCatalogue.Name};
    }
}


