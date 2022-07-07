using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using MDCourseProject.MDCourseSystem.MDCatalogues;
using Microsoft.Win32;

namespace MDCourseProject.MDCourseSystem.MDSubsystems
{
    public class DivisionsSubsystem:ISubsystem
    {
        private int _catalogueIndex;

        public DivisionsSubsystem()
        {
            CatalogueIndex = 0;
            DivisionsCatalogue = new DivisionsCatalogue();
            SendRequestsCatalogue = new SendRequestsCatalogue();
        }

        public DivisionsCatalogue DivisionsCatalogue { get; }

        public SendRequestsCatalogue SendRequestsCatalogue { get; }

        public void LoadFirstCatalogue(string filePath)
        {
            if (!filePath.EndsWith("Division.txt"))
            {
                MessageBox.Show("Некорректный файл для справочника Подразделения", "Ошибка!", MessageBoxButton.OK);
                throw new Exception("Incorrect file!");
            }
            DivisionsCatalogue.Load(filePath);
        }

        public void LoadSecondCatalogue(string filePath)
        {
            if (!filePath.EndsWith("SendRequest.txt"))
            {
                MessageBox.Show("Некорректный файл для справочника Отправленные заявки", "Ошибка!", MessageBoxButton.OK);
                throw new Exception("Incorrect file!");
            }
            SendRequestsCatalogue.Load(filePath);
        }

        public bool MakeReport(string[] data)
        {
            var saveReportDialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения отчета по подсистеме Подразделения",
                FileName ="Отчет: Заявки по услуге",
                Filter = "Text files (*.txt)|*.txt",
            };
        
            if (saveReportDialog.ShowDialog() == true)
            {
                var reportResults = new List<SendRequest>();

                if (DivisionsCatalogue.DivisionsByArea.Contains(data[3]) && SendRequestsCatalogue.SendRequestsByService.TryGetValuesList(data[0], out var list))
                {
                    var dateFrom = DateTime.Parse(data[1]);
                    var dateTo = DateTime.Parse(data[2]);
                    
                    foreach (var send in list)
                    {
                        var sendDate = DateTime.Parse(send.Date);
                        if (sendDate.CompareTo(dateFrom)>=0 && sendDate.CompareTo(dateTo)<=0 && string.Compare(send.Division.Area, data[3], StringComparison.OrdinalIgnoreCase)==0)
                        {
                            reportResults.Add(send);
                        }
                    }
                }

                var writer = new StreamWriter(saveReportDialog.FileName);
                
                foreach (var result in reportResults)
                    writer.WriteLine(result.ToString());
                
                if (reportResults.Count == 0)
                    writer.WriteLine("Не было найдено ни одной записи удовлетворяющей условиям!");
                
                writer.Close();
                    
                //Открывает итоговый текстовый файл для просмотра
                Process.Start(saveReportDialog.FileName);

                return true;
            }
            
            return false;
        }

        public DataAnalyser BuildReportWindow(Grid mainGrid)
        {
            mainGrid.RowDefinitions.Clear();
            CommonWindowGenerator.GenerateRowsInGrid(mainGrid, 3);

            var tBoxes = CommonWindowGenerator.CreateInputBetweenField(mainGrid, "Дата:", 2);
            var tList = new TextBox[]
            {
                CommonWindowGenerator.CreateInputField(mainGrid, "Услуга:", 0),
                tBoxes[0], //Дата от которой
                tBoxes[1], //Дата до которой
                CommonWindowGenerator.CreateInputField(mainGrid, "Район:", 4)
            };
            return new ReportDivisionsAnalyser(tList);
        }

        public int CatalogueIndex
        {
            get => _catalogueIndex;
            set
            {
                if (value < 0) value = 0;
                _catalogueIndex = value;

                //Этот вывод в консоль так... для красоты)
                MDDebugConsole.WriteLine(_catalogueIndex == 0 ? "Выбран справочник Подразделения" : "Выбран справочник Отправленные заявки");
            }
        }

        public Catalogue Catalogue => _catalogueIndex == 0 ? DivisionsCatalogue : SendRequestsCatalogue;

        public IEnumerable<string> CataloguesNames => new[]{DivisionsCatalogue.Name, SendRequestsCatalogue.Name};

        public void LoadDefaultFirstCatalogue()
        {
            LoadFirstCatalogue("DefaultFiles/divisions_default.txt");
        }

        public void LoadDefaultSecondCatalogue()
        {
            LoadSecondCatalogue("DefaultFiles/sendrequests_default.txt");
        }
    }
}


