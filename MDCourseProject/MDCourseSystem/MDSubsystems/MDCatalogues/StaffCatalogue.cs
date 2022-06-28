using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;
using MDCourseProject.AppWindows.WindowsBuilder;

namespace MDCourseProject.MDCourseSystem.MDCatalogues
{
    
    public class StaffCatalogue:Catalogue
    {
        public DynamicHashTable<StaffNameAndOccupation, StaffInfo> _staffTable;
        public RRBTree<WorkPlace, StaffInfo> _workplaceTree;
        private RRBTree<Occupation, StaffInfo> _occupationTree;
        private List<StaffInfo> _staffInfo;

        public StaffCatalogue()
        {
            _staffInfo = new List<StaffInfo>();
            _occupationTree = new RRBTree<Occupation, StaffInfo>();
            _workplaceTree = new RRBTree<WorkPlace, StaffInfo>();
            _staffTable = new DynamicHashTable<StaffNameAndOccupation, StaffInfo>
            {
                FirstHashFunc = key =>
                {
                    var mult = key * (Math.Sqrt(5) - 1) / 2;
                    var doublePart = mult - Math.Truncate(mult);
                    return (int)(_staffTable.GetCapacity() * doublePart);
                },
                
                SecondHashFunc = key =>
                {
                    key *= key;
                    var keyCountDigit = Count.CountDigit(key);
                    var howManyDigit = Count.CountDigit(_staffTable.GetCapacity());
                    key /= keyCountDigit == 1 ?  (int)Math.Pow(10, keyCountDigit / 2) : (int)Math.Pow(10, keyCountDigit / howManyDigit);
                    key %= (int)Math.Pow(10, howManyDigit);
                    return ++key;
                }
            };
        }
        public override void Add(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.GetFullName(), staffInfo.GetOccupation());
            if (_staffTable.Contains(keyToStaffTable, staffInfo) || _staffTable.ContainsKey(keyToStaffTable)) return;
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.GetOccupation(), staffInfo.GetDistrict());
            _occupationTree.Add(staffInfo.GetOccupation(), staffInfo);
            _workplaceTree.Add(keyToWorkPlaceTree, staffInfo);
            _staffTable.Add(keyToStaffTable, staffInfo);
            _staffInfo.Add(staffInfo);
        }

        public override void Remove(string[] data)
        {
            var staffInfo = new StaffInfo(new FullName(data[0]), new Occupation(data[1]), new District(data[2]));
            var keyToStaffTable = new StaffNameAndOccupation(staffInfo.GetFullName(), staffInfo.GetOccupation());
            var keyToWorkPlaceTree = new WorkPlace(staffInfo.GetOccupation(), staffInfo.GetDistrict());
            _occupationTree.Delete(staffInfo.GetOccupation(), staffInfo);
            _workplaceTree.Delete(keyToWorkPlaceTree, staffInfo);
            _staffTable.Remove(keyToStaffTable, staffInfo);
            _staffInfo.Remove(staffInfo);
        }

        public override void Find(DataGrid mainDataGrid, string[] data)
        {
            throw new NotImplementedException();
        }

        public override void PrintDataToGrid(DataGrid mainDataGrid)
        {
            throw new NotImplementedException();
        }

        public override void Load(string filePath)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        public override DataAnalyser BuildAddValuesWindow(Grid mainGrid)
        {
            return new AddValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО", "Должность", "Район"));
        }

        public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            return new RemoveValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО", "Должность", "Район"));
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            return new SearchValuesStaffAnalyser(CommonWindowGenerator.CreateWindow(mainGrid, "ФИО", "Должность"));
        }

        public override string Name => "Сотрудники";
        
    }
}