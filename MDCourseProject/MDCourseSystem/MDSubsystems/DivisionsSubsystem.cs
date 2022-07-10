using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
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
                MessageBox.Show("Некорректный файл для справочника Подразделения!", "Ошибка!", MessageBoxButton.OK);
                throw new Exception("Incorrect file!");
            }
            DivisionsCatalogue.Load(filePath);
        }

        public void LoadSecondCatalogue(string filePath)
        {
            if (!filePath.EndsWith("SendRequest.txt"))
            {
                MessageBox.Show("Некорректный файл для справочника Отправленные заявки!", "Ошибка!", MessageBoxButton.OK);
                throw new Exception("Incorrect file!");
            }
            SendRequestsCatalogue.Load(filePath);
        }

        public bool MakeReport(string[] data)
        {
            var saveReportDialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения отчета \"Заявки подразделения\"",
                FileName ="Отчет \"Заявки подразделения\"",
                Filter = "Text files (*.txt)|*.txt",
            };
        
            if (saveReportDialog.ShowDialog() == true)
            {
                var reportResults = new List<SendRequest>();

                var dateFrom = DateTime.Parse(data[2]);
                var dateTo = DateTime.Parse(data[3]);

                // 0 - Тип подразделения
                // 1 - Район
                // 2 - Дата от
                // 3 - Дата до
                
                if (DivisionsCatalogue.DivisionsByArea.TryGetValuesList(data[1], out var areaList))
                {
                    foreach (var divisionByArea in areaList)
                    {
                        if (divisionByArea.Type != data[0]) continue;
                        
                        if (
                            SendRequestsCatalogue.SendRequestsTree.TryGetValuesList(
                                key: new DivisionNameAndArea(divisionByArea.Name, divisionByArea.Area),
                                list: out var sendList
                            )
                        ) {
                            foreach (var send in sendList)
                            {
                                var sendDate = DateTime.Parse(send.Date);
                                if (sendDate.CompareTo(dateFrom)>=0 && sendDate.CompareTo(dateTo)<=0)
                                {
                                    reportResults.Add(send);
                                }
                            }
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
                    reportResults.Sort((request, other) =>
                    {
                        var date1 = DateTime.Parse(request.Date);
                        var date2 = DateTime.Parse(other.Date);
                        return date1.CompareTo(date2);
                    });
                    
                    foreach (var result in reportResults)
                        writer.WriteLine(result.ToString());
                }

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

            var tBoxes = CommonWindowGenerator.CreateInputBetweenField(mainGrid, "Дата:", 4);
            var tList = new TextBox[]
            {
                CommonWindowGenerator.CreateInputField(mainGrid, "Тип подразделения:", 0),
                CommonWindowGenerator.CreateInputField(mainGrid, "Район:", 2),
                tBoxes[0], //Дата от которой
                tBoxes[1] //Дата до которой
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
            }
        }

        public Catalogue Catalogue => _catalogueIndex == 0 ? DivisionsCatalogue : SendRequestsCatalogue;

        public IEnumerable<string> CataloguesNames => new[]{DivisionsCatalogue.Name, SendRequestsCatalogue.Name};

        public void LoadDefaultFirstCatalogue()
        {
            LoadFirstCatalogue("DefaultFiles/default_Division.txt");
        }

        public void LoadDefaultSecondCatalogue()
        {
            LoadSecondCatalogue("DefaultFiles/default_SendRequest.txt");
        }
    }
}


