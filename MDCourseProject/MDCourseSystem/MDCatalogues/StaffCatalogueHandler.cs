using System;
using FundamentalStructures;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDCourseProject.MDCourseSystem.MDCatalogues.Divisions
{
    public class FullName: IComparable<FullName>
    {
        private readonly string _surname;
        private readonly string _name;
        private readonly string _patronymic;

        public FullName(string fullname)
        {
            var fnInfo = fullname.Split();
            (_surname, _name, _patronymic) = (fnInfo[0], fnInfo[1], fnInfo[2]);
        }

        public int CompareTo(FullName other)
        {
            var bySurname = string.Compare(_surname, other._surname, StringComparison.OrdinalIgnoreCase);
            if (bySurname != 0) return bySurname;
            var byName = string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
            return byName != 0 ? byName : string.Compare(_patronymic, other._patronymic, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return string.Join(" ", _surname, _name, _patronymic);
        }

        public override int GetHashCode()
        {
            var sASCII = Encoding.ASCII.GetBytes(_surname);
            var nASCII = Encoding.ASCII.GetBytes(_name);
            var pASCII = Encoding.ASCII.GetBytes(_patronymic);
            var hash = sASCII.Aggregate(0, (current, elem) => current + elem);
            hash = nASCII.Aggregate(hash, (current, elem) => current + elem);
            return pASCII.Aggregate(hash, (current, elem) => current + elem);
        }
    }

    public class Occupation: IComparable<Occupation>
    {
        private readonly string _occupation;

        public Occupation(string occupation) => _occupation = occupation;
        
        public int CompareTo(Occupation other) => string.Compare(_occupation, other._occupation, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Encoding.ASCII.GetBytes(_occupation).Aggregate(0, (curr, elem) => curr + elem);

        public override string ToString() => _occupation;
    }

    public class District: IComparable<District>
    {
        private readonly string _district;

        public District(string district) => _district = district;

        public int CompareTo(District other) => string.Compare(_district, other._district, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => _district;
    }
    
    public class StaffCatalogueHandler
    {
        private class StaffInfo:IComparable<StaffInfo>
        {
            private readonly FullName _staffName;
            private readonly Occupation _occupation;
            private readonly District _district;

            public StaffInfo(FullName fullname, Occupation occupation, District district)
            {
                (_staffName, _occupation, _district) = (fullname, occupation, district);
            }

            public int CompareTo(StaffInfo other)
            {
                var byFullName = _staffName.CompareTo(other._staffName);
                if (byFullName != 0) return byFullName;
                var byOccupation = _occupation.CompareTo(other._occupation);
                return byOccupation != 0 ? byOccupation : _district.CompareTo(other._district);
            }

            public override string ToString()
            {
                return string.Join(" ", _staffName, _occupation, _district);
            }

            public FullName GetFullName() => _staffName;

            public Occupation GetOccupation() => _occupation;

            public District GetDistrict() => _district;
        }

        private class Staff:IComparable<Staff>
        {
            private readonly FullName _staffName;
            private readonly Occupation _occupation;

            public Staff(FullName fullname, Occupation occupation)
            {
                (_staffName, _occupation) = (fullname, occupation);
            }

            public int CompareTo(Staff other)
            {
                var byFullName = _staffName.CompareTo(other._staffName);
                return byFullName != 0 ? byFullName : _occupation.CompareTo(other._occupation);
            }
            
            public override int GetHashCode()
            {
                return _staffName.GetHashCode() + _occupation.GetHashCode();
            }
        }

        private class Work:IComparable<Work>
        {
            private Occupation _occupation;
            private District _district;

            public Work(Occupation occupation, District district)
            {
                (_occupation, _district) = (occupation, district);
            }

            public int CompareTo(Work other)
            {
                var byOccupation = _occupation.CompareTo(other._occupation);
                return byOccupation != 0 ? byOccupation : _district.CompareTo(other._district);
            }
        }
        
        
        private List<StaffInfo> _view;
        private DynamicHashTable<Staff, StaffInfo> _hashTable;
        private RRBTree<Work, StaffInfo> _tree1;
        private RRBTree<Occupation, StaffInfo> _tree2;
    }
}