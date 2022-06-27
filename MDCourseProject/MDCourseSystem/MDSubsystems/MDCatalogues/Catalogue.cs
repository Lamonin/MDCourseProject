using System;
using System.Collections;
using System.IO;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using MDCourseProject.AppWindows.WindowsBuilder;
using Microsoft.Win32;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public abstract class Catalogue
{
    public abstract void Add(string[] data);
    public abstract void Remove(string[] data);
    public abstract void Find(DataGrid mainDataGrid, string[] data);
    public abstract void PrintDataToGrid(DataGrid mainDataGrid);
    
    protected static void PrintDataToGrid(DataGrid mainDataGrid, IEnumerable itemsSource, string[] headers)
    {
        mainDataGrid.ItemsSource = null;
        mainDataGrid.Columns.Clear();
        mainDataGrid.ItemsSource = itemsSource;
        mainDataGrid.Items.Refresh();
        CommonWindowGenerator.CreateHeadersInDataGrid(mainDataGrid, headers);
    }
    
    public abstract void Load(string filePath);
    public abstract void Save();
    protected bool OpenSaveCatalogueDialog(string name, out string savePath)
    {
        var saveDialog = new SaveFileDialog
        {
            FileName = name + "_Справочник",
            Filter = "Text files (*.txt)|*.txt",
        };
        
        if (saveDialog.ShowDialog() == true)
        {
            savePath = saveDialog.FileName;
            return true;
        }
        
        savePath = default;
        return false;
    }
    public abstract DataAnalyser BuildAddValuesWindow(Grid mainGrid);
    public abstract DataAnalyser BuildRemoveValuesWindow(Grid mainGrid);
    public abstract DataAnalyser BuildSearchValuesWindow(Grid mainGrid);
    public abstract string Name { get; }
}