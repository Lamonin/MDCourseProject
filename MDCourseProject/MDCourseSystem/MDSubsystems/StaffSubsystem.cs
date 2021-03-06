using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using MDCourseProject.MDCourseSystem.MDCatalogues;
using Microsoft.Win32;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class StaffSubsystem:ISubsystem
{
    private StaffCatalogue _staffCatalogue;
    private DocumentCatalogue _documentCatalogue;

    public StaffSubsystem()
    {
        _catalogueIndex = 0;
        _documentCatalogue = new DocumentCatalogue();
        _staffCatalogue = new StaffCatalogue();
    }

    public void LoadDefaultFirstCatalogue()
    {
        LoadFirstCatalogue("DefaultFiles/Staff.txt");
    }

    public void LoadDefaultSecondCatalogue()
    {
        LoadSecondCatalogue("DefaultFiles/Document.txt");
    }
     public void LoadFirstCatalogue(string filePath)
     {
         var path = filePath.Split('/', '\\');
         if(path[path.Length - 1].StartsWith("Staff") && path[path.Length - 1].EndsWith(".txt"))
             _staffCatalogue.Load(filePath);
         else
         {
             MessageBox.Show("Название файла для справочника \" Сотрудники \" некорректно! Файл должен начинаться со слова <Staff>!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
         }
    }

    public void LoadSecondCatalogue(string filePath)
    {
        var path = filePath.Split('/', '\\');
        if(path[path.Length - 1].StartsWith("Document") && path[path.Length - 1].EndsWith(".txt"))
            _documentCatalogue.Load(filePath);
        else
        {
            MessageBox.Show("Название файла для справочника \" Документы \" некорректно!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public bool MakeReport(string[] data)
    {
        var saveReportDialogWindow = new SaveFileDialog
        {
            Title = "Выберите место для сохранения отчета <Документы сотрудника>",
            FileName = "Отчет Документы сотрудника",
            Filter = "Text files (*.txt)|*.txt"
        };
        
        if (saveReportDialogWindow.ShowDialog() == true)
        {
            var report = new List<StaffInfo>();
            var searchKey = new Document(data[0]);
            if (_documentCatalogue.DocumentTree.ContainKey(searchKey))
            {
                var documentInfo = _documentCatalogue.DocumentTree.Find(searchKey, out var step);
                MDDebugConsole.WriteLine($"Ключ {searchKey} найден за {step} операцию(-и) сравнений");
                var district = new District(data[1]);
                foreach (var document in documentInfo)
                {
                    var searchWork = new WorkPlace(document.Occupation, district);
                    if (_staffCatalogue.WorkplaceTree.ContainKey(searchWork))
                    {
                        var staffInfo = _staffCatalogue.WorkplaceTree.Find(searchWork, out var step1);
                        MDDebugConsole.WriteLine($"Ключ {searchWork} найден за {step1} операцию(-и) сравнений");
                        foreach (var staff in staffInfo)
                            report.Add(staff);
                    }
                }
                var output = new StreamWriter(saveReportDialogWindow.FileName);
                foreach (var staff in report)
                    output.WriteLine(staff.ToString());
                output.Close();

                Process.Start(saveReportDialogWindow.FileName);
            }
            else return false;
            return true;
        }
        return false;
    }

    public DataAnalyser BuildReportWindow(Grid mainGrid)
    {
        return new ReportStaffSubSystemAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:", "Район:"));
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

    public StaffCatalogue StaffCatalogue => _staffCatalogue;

    public DocumentCatalogue DocumentCatalogue => _documentCatalogue;
    
    public Catalogue Catalogue => _catalogueIndex == 0 ? _staffCatalogue : _documentCatalogue;

    public IEnumerable<string> CataloguesNames => new[] {"Сотрудники", "Документы"};
}