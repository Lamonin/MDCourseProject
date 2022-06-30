using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues
{
    public class DocumentCatalogue:Catalogue
    {
        private RRBTree<Document, DocumentInfo> _documentTree;
        private RRBTree<DivisionName, DocumentInfo> _divisionNameTree;
        private RRBTree<Occupation, DocumentInfo> _occupationTree;
        private List<DocumentInfo> _documentInfo;

        public RRBTree<Document, DocumentInfo> DocumentTree => _documentTree;
        public RRBTree<DivisionName, DocumentInfo> DivisionName => _divisionNameTree;
        public RRBTree<Occupation, DocumentInfo> OccupationTree => _occupationTree;

        public DocumentCatalogue()
        {
            _documentInfo = new List<DocumentInfo>();
            _documentTree = new RRBTree<Document, DocumentInfo>();
            _occupationTree = new RRBTree<Occupation, DocumentInfo>();
            _divisionNameTree = new RRBTree<DivisionName, DocumentInfo>();
        }
        
        public override void Add(string[] data)
        {
            var documentInfo = new DocumentInfo(new Document(data[0]), new Occupation(data[1]), new DivisionName(data[2]));
            var keyToDocumentTree = documentInfo.Document;
            if (_documentTree.Contains(keyToDocumentTree, documentInfo))
            {
                MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var keyToDivision = documentInfo.DivisionName;
            var keyToOccupation = documentInfo.Occupation;
            _documentInfo.Add(documentInfo);
            _documentTree.Add(keyToDocumentTree, documentInfo);
            _occupationTree.Add(keyToOccupation, documentInfo);
            _divisionNameTree.Add(keyToDivision, documentInfo);
        }

        public override void Remove(string[] data)
        {
            var documentInfo = new DocumentInfo(new Document(data[0]), new Occupation(data[1]), new DivisionName(data[2]));
            var keyToDocumentTree = new Document(data[0]);
            var keyToDivision = new DivisionName(data[2]);
            var keyToOccupation = new Occupation(data[1]);
            _documentTree.Delete(keyToDocumentTree, documentInfo);
            _occupationTree.Delete(keyToOccupation, documentInfo);
            _divisionNameTree.Delete(keyToDivision, documentInfo);
            _documentInfo.RemoveAll(document => document.CompareTo(documentInfo) == 0);
        }

        public override void Find(DataGrid mainDataGrid, string[] data)
        {
            var findKey = new Document(data[0]);
            var info = _documentTree.GetValue(findKey);
            if(info != null)
                PrintDataToGrid(mainDataGrid, info, new []{"Тип документа", "Должность", "Подразделение"});
            else
            {
                MessageBox.Show("Элемент не найден!", "Предупрежедние", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public override void PrintDataToGrid(DataGrid mainDataGrid)
        {
            PrintDataToGrid(mainDataGrid, _documentInfo, new []{"Тип документа", "Должность", "Подразделение"});
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
                
                foreach(var document in _documentInfo)
                    output.WriteLine(string.Join("|", document.Document, document.Occupation, document.DivisionName));
                output.Close();
            }
        }

        public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
        {
            return new AddValuesDocumentsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:", "Должность:", "Подразделение:"));
        }

        public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            return new RemoveValuesDocumentAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:", "Должность:", "Подразделение:"));
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            return new SearchValuesDocumentAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:"));
        }

        public override string Name => "Документы";
    }
}