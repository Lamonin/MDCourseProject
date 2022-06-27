using System;
using System.Collections.Generic;
using System.Windows.Controls;
using MDCourseProject.AppWindows.DataAnalysers;
using FundamentalStructures;

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
            _staffTable = new DynamicHashTable<StaffNameAndOccupation, StaffInfo>
            {
                FirstHashFunc = key =>
                {
                    var mult = key * (Math.Sqrt(5) - 1) / 2;
                    var doublePart = mult - Math.Truncate(mult);
                    return (int)(_staffTable.GetCapacity() * doublePart);
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
            throw new NotImplementedException();
        }

        public override DataAnalyser BuildRemoveValuesWindow(Grid mainGrid)
        {
            throw new NotImplementedException();
        }

        public override DataAnalyser BuildSearchValuesWindow(Grid mainGrid)
        {
            throw new NotImplementedException();
        }

        public override string Name { get; }
    }
}