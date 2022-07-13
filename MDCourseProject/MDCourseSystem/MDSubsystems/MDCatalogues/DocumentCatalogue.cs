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
        private List<DocumentInfo> _documentInfo;

        public RRBTree<Document, DocumentInfo> DocumentTree { get; }

        public RRBTree<DivisionName, DocumentInfo> DivisionName { get; }

        public RRBTree<Occupation, DocumentInfo> OccupationTree { get; }

        public DocumentCatalogue()
        {
            _documentInfo = new List<DocumentInfo>();
            DocumentTree = new RRBTree<Document, DocumentInfo>();
            OccupationTree = new RRBTree<Occupation, DocumentInfo>();
            DivisionName = new RRBTree<DivisionName, DocumentInfo>();
        }
        
        public override void Add(string[] data)
        {
            var documentInfo = new DocumentInfo(new Document(data[0]), new Occupation(data[1]), new DivisionName(data[2]));
            var keyToDocumentTree = documentInfo.Document;
            if (DocumentTree.Contains(keyToDocumentTree, documentInfo))
            {
                MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var keyToDivision = documentInfo.DivisionName;
            var keyToOccupation = documentInfo.Occupation;
            _documentInfo.Add(documentInfo);
            DocumentTree.Add(keyToDocumentTree, documentInfo);
            OccupationTree.Add(keyToOccupation, documentInfo);
            DivisionName.Add(keyToDivision, documentInfo);
        }

        public override void Remove(string[] data)
        {
            var documentInfo = new DocumentInfo(new Document(data[0]), new Occupation(data[1]), new DivisionName(data[2]));
            var keyToDocumentTree = new Document(data[0]);
            var keyToDivision = new DivisionName(data[2]);
            var keyToOccupation = new Occupation(data[1]);
            DocumentTree.Delete(keyToDocumentTree, documentInfo);
            OccupationTree.Delete(keyToOccupation, documentInfo);
            DivisionName.Delete(keyToDivision, documentInfo);
            _documentInfo.RemoveAll(document => document.CompareTo(documentInfo) == 0);
        }

        public override void Find(DataGrid mainDataGrid, string[] data)
        {
            var findKey = new Document(data[0]);
            var info = DocumentTree.Find(findKey, out var step);
            if(info != null)
            {
                MDDebugConsole.WriteLine($"Ключ {findKey} найден за {step} операцию(-ии) сравнений");
                PrintDataToGrid(mainDataGrid, info, new[] {"Тип документа", "Должность сотрудника", "Название подразделения"});
            }
            else
            {
                MessageBox.Show("Элемент не найден!", "Предупрежедние", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public override void PrintDataToGrid(DataGrid mainDataGrid)
        {
            PrintDataToGrid(mainDataGrid, _documentInfo, new []{"Тип документа", "Должность сотрудника", "Название подразделения"});
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
            if (OpenSaveCatalogueDialog("Document", out var filePath))
            {
                var file = filePath.Split('/', '\\');
                if (file[file.Length - 1].EndsWith(".txt") && file[file.Length - 1].StartsWith("Document"))
                {
                    var output = new StreamWriter(filePath);
                    output.Flush();

                    foreach (var document in _documentInfo)
                        output.WriteLine(string.Join("|", document.Document, document.Occupation, document.DivisionName));
                    output.Close();
                }
                else
                {
                    MessageBox.Show("Некорректное название файла для сохранения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
        {
            return new AddValuesDocumentsAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа", "Должность сотрудника", "Название подразделения"));
        }

        public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            return new RemoveValuesDocumentAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа", "Должность сотрудника", "Название подразделения"));
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            return new SearchValuesDocumentAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "Тип документа:"));
        }

        public override string PrintData()
        {
            return "Дерево (поиск, добавление, удаление):\n \n" + DocumentTree.PrintTree() 
                    + "\n Дерево (целостность \" Документы \" - \" Сотрудники \" по должности сотрудника):\n \n" + OccupationTree.PrintTree() 
                   + "\n Дерево (целостность \" Документы \" - \" Подразделения \" по названию подразделения):\n \n" + DivisionName.PrintTree();
        }

        public override string Name => "Документы";
    }
}