using System;
using FundamentalStructures;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDCourseProject.MDCourseSystem.MDCatalogues.Divisions
{
    public class FullName: IComparable<FullName>
    {
        private string _surname;
        private string _name;
        private string _patronomyc;

        public FullName(string fullname)
        {
            var fnInfo = fullname.Split();
            (_surname, _name, _patronomyc) = (fnInfo[0], fnInfo[1], fnInfo[2]);
        }

        public int CompareTo(FullName other)
        {
            var bySurname = string.Compare(_surname, other._surname, StringComparison.OrdinalIgnoreCase);
            if (bySurname != 0) return bySurname;
            var byName = string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
            return byName != 0 ? byName : string.Compare(_patronomyc, other._patronomyc, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return string.Join(" ", _surname, _name, _patronomyc);
        }

        public override int GetHashCode()
        {
            var sASCII = Encoding.ASCII.GetBytes(_surname);
            var nASCII = Encoding.ASCII.GetBytes(_name);
            var pASCII = Encoding.ASCII.GetBytes(_patronomyc);
            var hash = sASCII.Aggregate(0, (current, elem) => current + elem);
            hash = nASCII.Aggregate(hash, (current, elem) => current + elem);
            return pASCII.Aggregate(hash, (current, elem) => current + elem);
        }
    }
    public class StaffSystem
    {
        
    }
}