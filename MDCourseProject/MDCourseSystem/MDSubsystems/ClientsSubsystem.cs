﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;

namespace MDCourseProject.MDCourseSystem.MDSubsystems;

public class ClientsSubsystem:ISubsystem
{
    public void Add(string[] data)
    {
    }

    public void PrintDataInGrid(DataGrid mainDataGrid)
    {
    }

    public DataAnalyser BuildAddValuesWindow(Grid mainGrid)
    {
        return null;
    }

    public DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
    {
        return null;
    }

    public DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
    {
        return null;
    }

    public event Action OnCatalogueValuesUpdated;

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

    public string CurrentCatalogueName => CatalogueIndex == 0 ? "Клиенты": "Обращения";

    public IEnumerable<string> CataloguesNames => new []{"Клиенты", "Обращения"};
}