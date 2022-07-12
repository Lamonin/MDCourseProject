using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues
{
    
    public class StaffCatalogue:Catalogue
    {
        private List<StaffInfo> _staffInfo;
        
        public DynamicHashTable<StaffNameAndOccupation, StaffInfo> StaffTable { get; }

        public RRBTree<WorkPlace, StaffInfo> WorkplaceTree { get; }

        public RRBTree<Occupation, StaffInfo> OccupationTree { get; }
        public StaffCatalogue()
        {
            _staffInfo = new List<StaffInfo>();
            OccupationTree = new RRBTree<Occupation, StaffInfo>();
            WorkplaceTree = new RRBTree<WorkPlace, StaffInfo>();
            StaffTable = new DynamicHashTable<StaffNameAndOccupation, StaffInfo>();
            StaffTable.FirstHashFunc = key =>
            {
                var mult = key.GetHashCode() * (Math.Sqrt(5) - 1) / 2;
                var doublePart = mult - Math.Truncate(mult);
                return (int)(StaffTable.GetCapacity() * doublePart);
            };
            StaffTable.SecondHashFunc = key => (2 * key.GetHashCode() + 1) % StaffTable.GetCapacity();
        }
        public override void Add(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.FullName, staffInfo.Occupation);
            if (StaffTable.Contains(keyToStaffTable, staffInfo))
            {
                MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (StaffTable.ContainsKey(keyToStaffTable))
            {
                MessageBox.Show($"Ключ {keyToStaffTable} неуникален", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.Occupation, staffInfo.District);
            OccupationTree.Add(staffInfo.Occupation, staffInfo);
            WorkplaceTree.Add(keyToWorkPlaceTree, staffInfo);
            StaffTable.Add(keyToStaffTable, staffInfo);
            _staffInfo.Add(staffInfo);
        }

        public override void Remove(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.FullName, staffInfo.Occupation);
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.Occupation, staffInfo.District);
            OccupationTree.Delete(staffInfo.Occupation, staffInfo);
            WorkplaceTree.Delete(keyToWorkPlaceTree, staffInfo);
            StaffTable.Remove(keyToStaffTable, staffInfo);
            _staffInfo.RemoveAll(staff => staff.CompareTo(staffInfo) == 0);
            if(!OccupationTree.ContainKey(staffInfo.Occupation))
            {
                var result = MDSystem.staffSubsystem.DocumentCatalogue.OccupationTree.GetValue(staffInfo.Occupation);
                if(result != null)
                    foreach (var delete in result)
                    {
                        MDSystem.staffSubsystem.DocumentCatalogue.Remove(new[]
                            {delete.Document.ToString(), delete.Occupation.ToString(), delete.DivisionName.ToString()});
                    }
            }
            var staffInApplicationCatalogue = new Staff(staffInfo.FullName.Surname,staffInfo.FullName.Name,  staffInfo.FullName.Patronymic, staffInfo.Occupation.ToString());
            if (MDSystem.clientsSubsystem._applications._tree != null && MDSystem.clientsSubsystem._applications._tree.IsKeyExist(staffInApplicationCatalogue))
            {
                var applicationInfo = MDSystem.clientsSubsystem._applications._tree
                    .GetLeaf(MDSystem.clientsSubsystem._applications._tree.m_root, staffInApplicationCatalogue).valList;
                if (applicationInfo != null)
                {
                    var pointer = applicationInfo.head;
                    while (pointer != null)
                    {
                        var tmp = pointer.pNext;
                        MDSystem.clientsSubsystem._applications.Remove(new []{pointer.pData.staff.StaffName, pointer.pData.staff.StaffSurname, 
                            pointer.pData.staff.StaffPatronymic, pointer.pData.staff.StaffOccupation, pointer.pData.ClientName, pointer.pData.ClientSurname,
                            pointer.pData.ClientPatronymic, pointer.pData.ClientTelephone, pointer.pData.Date.ToString(CultureInfo.InvariantCulture)
                        } );
                        pointer = tmp;
                    }
                }
            }
        }

        public override void Find(DataGrid mainDataGrid, string[] data)
        {
            var keyToStaffTable = new StaffNameAndOccupation(new FullName(data[0]), new Occupation(data[1]));
            StaffTable.TryGetValue(keyToStaffTable, out var res, out var steps);
            if (res != null)
            {
                MDDebugConsole.Write($"Ключ {keyToStaffTable} найден за {steps} операцию(-и) сравнений ");
                PrintDataToGrid(mainDataGrid, new List<StaffInfo> {res}, new[] {"ФИО", "Должность", "Район"});
            }
            else
                MessageBox.Show("Элемент не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public override void PrintDataToGrid(DataGrid mainDataGrid)
        {
            PrintDataToGrid(mainDataGrid, _staffInfo, new []{"ФИО", "Должность", "Район"});
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
            if (OpenSaveCatalogueDialog("Staff", out var filePath))
            {
                var output = new StreamWriter(filePath);
                output.Flush();
                
                foreach(var staff in _staffInfo)
                    output.WriteLine(string.Join("|", staff.FullName, staff.Occupation, staff.District));
                output.Close();
            }
        }

        public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
        {
            return new AddValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность:", "Район:"));
        }

        public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность:", "Район:"));
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            return new DataAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность:"));
        }

        public override string PrintData()
        {
            return "Хеш-таблица:\n \n" + StaffTable.ToStringWithStatuses()
                + "\nДерево (целостность \"Сотрудники\" - \"Документы\" по должности сотрудника):\n \n" + OccupationTree.PrintTree()
                + "\nДерево (формирование отчета):\n \n" + WorkplaceTree.PrintTree();
        }

        public override string Name => "Сотрудники";
        
    }
}