
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        _staffCatalogue.Load(filePath);
    }

    public void LoadSecondCatalogue(string filePath)
    {
        _documentCatalogue.Load(filePath);
    }

    public bool MakeReport(string[] data)
    {
        var saveReportDialogWindow = new SaveFileDialog
        {
            Title = "Выберите место для сохранения отчета <Документы сотрудника>",
            FileName = "Документы сотдрудника",
            Filter = "Text files (*.txt)"
        };

        
        if (saveReportDialogWindow.ShowDialog() == true)
        {
            var report = new List<FullName>();
            var searchKey = new Document(data[0]);
            if (_documentCatalogue.DocumentTree.ContainKey(searchKey))
            {
                var documentInfo = _documentCatalogue.DocumentTree.GetValue(searchKey);
                var district = new District(data[1]);
                foreach (var document in documentInfo)
                {
                    var searchWork = new WorkPlace(document.Occupation, district);
                    var staffInfo = _staffCatalogue.WorkplaceTree.GetValue(searchWork);
                    foreach (var staff in staffInfo)
                    {
                        report.Add(staff.FullName);
                    }
                }
                report.Sort();
                var output = new StreamWriter(saveReportDialogWindow.FileName);
                foreach (var staffFullName in report)
                    output.WriteLine(staffFullName.ToString());
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