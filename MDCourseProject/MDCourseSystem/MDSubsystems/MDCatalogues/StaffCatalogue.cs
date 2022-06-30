using System;
using System.Collections.Generic;
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
        private DynamicHashTable<StaffNameAndOccupation, StaffInfo> _staffTable;
        private RRBTree<WorkPlace, StaffInfo> _workplaceTree;
        private RRBTree<Occupation, StaffInfo> _occupationTree;
        private List<StaffInfo> _staffInfo;
        public StaffCatalogue()
        {
            _staffInfo = new List<StaffInfo>();
            _occupationTree = new RRBTree<Occupation, StaffInfo>();
            _workplaceTree = new RRBTree<WorkPlace, StaffInfo>();
            _staffTable = new DynamicHashTable<StaffNameAndOccupation, StaffInfo>();
            _staffTable.FirstHashFunc = key =>
            {
                var mult = key * (Math.Sqrt(5) - 1) / 2;
                var doublePart = mult - Math.Truncate(mult);
                return (int)(_staffTable.GetCapacity() * doublePart);
            };
            _staffTable.SecondHashFunc = key =>
            {
                key *= key;
                var keyCountDigit = Count.CountDigit(key);
                var howManyDigit = Count.CountDigit(_staffTable.GetCapacity());
                key /= keyCountDigit == 1 ?  (int)Math.Pow(10, keyCountDigit / 2) : (int)Math.Pow(10, keyCountDigit / howManyDigit);
                key %= (int)Math.Pow(10, howManyDigit);
                return ++key;
            };
        }
        public override void Add(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.FullName, staffInfo.Occupation);
            if (_staffTable.Contains(keyToStaffTable, staffInfo) || _staffTable.ContainsKey(keyToStaffTable))
            {
                MessageBox.Show("Элемент существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.Occupation, staffInfo.District);
            _occupationTree.Add(staffInfo.Occupation, staffInfo);
            _workplaceTree.Add(keyToWorkPlaceTree, staffInfo);
            _staffTable.Add(keyToStaffTable, staffInfo);
            _staffInfo.Add(staffInfo);
            MDDebugConsole.WriteLine($"Добавление в таблицу по ключу: {keyToStaffTable}; Первичная ХФ: {_staffTable.FirstHashFunc(keyToStaffTable.GetHashCode())}; Вторичная ХФ: {_staffTable.SecondHashFunc(keyToStaffTable.GetHashCode())}");
        }

        public override void Remove(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.FullName, staffInfo.Occupation);
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.Occupation, staffInfo.District);
            _occupationTree.Delete(staffInfo.Occupation, staffInfo);
            _workplaceTree.Delete(keyToWorkPlaceTree, staffInfo);
            _staffTable.Remove(keyToStaffTable, staffInfo);
            _staffInfo.RemoveAll(staff => staff.CompareTo(staffInfo) == 0);
            if(!_occupationTree.ContainKey(staffInfo.Occupation))
            {
                var result = MDSystem.staffSubsystem.DocumentCatalogue.OccupationTree.GetValue(staffInfo.Occupation);
                if(result != null)
                    foreach (var delete in result)
                    {
                        MDSystem.staffSubsystem.DocumentCatalogue.Remove(new[]
                            {delete.Document.ToString(), delete.Occupation.ToString(), delete.DivisionName.ToString()});
                    }
            }
            //TODO удалить в справочниках "Отправленные заявки"
        }

        public override void Find(DataGrid mainDataGrid, string[] data)
        {
            var keyToStaffTable = new StaffNameAndOccupation(new FullName(data[0]), new Occupation(data[1]));
            _staffTable.TryGetValue(keyToStaffTable, out var res);
            if (res != null)
                PrintDataToGrid(mainDataGrid, new List<StaffInfo> {res}, new[] {"ФИО", "Должность", "Район"});
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
            if (OpenSaveCatalogueDialog(Name, out var filePath))
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
            return new RemoveValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность:", "Район:"));
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            return new SearchValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО:", "Должность:"));
        }

        public override string PrintData()
        {
            return string.Empty;
        }

        public override string Name => "Сотрудники";
        
        public DynamicHashTable<StaffNameAndOccupation, StaffInfo> StaffTable => _staffTable;
        
        public RRBTree<WorkPlace, StaffInfo> WorkplaceTree => _workplaceTree;
        
        public RRBTree<Occupation, StaffInfo> OccupationTree => _occupationTree;

    }
}