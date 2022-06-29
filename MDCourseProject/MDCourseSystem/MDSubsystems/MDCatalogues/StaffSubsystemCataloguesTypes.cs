using System;
using System.Linq;
using System.Text;

namespace MDCourseProject.MDCourseSystem.MDCatalogues
{
    public static class Count
    {
        public static int CountDigit(int num)
        {
            var count = 0;
            while (num > 0)
            {
                ++count;
                num /= 10;
            }
            return count;
        }
    }
    
    public class FullName:IComparable<FullName>
    {
        private readonly string _surname;
        private readonly string _name;
        private readonly string _patronymic;

        public FullName(string fullname)
        {
            var fInfo = fullname.Split();
            (_surname, _name, _patronymic) = (fInfo[0], fInfo[1], fInfo[2]);
        }
            
        public int CompareTo(FullName other)
        {
            var cmpSurname = string.Compare(_surname, other._surname, StringComparison.OrdinalIgnoreCase);
            if (cmpSurname != 0) return cmpSurname;
            var cmpName = string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
            return cmpName != 0 ? cmpName : string.Compare(_patronymic, other._patronymic, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => string.Join(" ", _surname, _name, _patronymic);

        public override int GetHashCode()
        {
            var codeSurname = Encoding.UTF8.GetBytes(_surname);
            var codeName = Encoding.UTF8.GetBytes(_name);
            var codePatronymic = Encoding.UTF8.GetBytes(_patronymic);
            var hash = codeSurname.Aggregate(0, (curr, elem) => curr + elem);
            hash += codeName.Aggregate(0, (curr, elem) => curr + elem);
            hash += codePatronymic.Aggregate(0, (curr, elem) => curr + elem);
            return hash;
        }
    }

    public class Occupation:IComparable<Occupation>
    {
        private readonly string _occupation;

        public Occupation(string occupation)
        {
            _occupation = occupation;
        }

        public int CompareTo(Occupation other) => string.Compare(_occupation, other._occupation, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => _occupation;

        public override int GetHashCode()
        {
            var codeOccupation = Encoding.UTF8 .GetBytes(_occupation);
            return codeOccupation.Aggregate(0, (curr, elem) => curr + elem);
        }
    }

    public class District:IComparable<District>
    {
        private readonly string _district;

        public District(string district)
        {
            _district = district;
        }

        public int CompareTo(District other) => string.Compare(_district, other._district, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => _district;
    }

    public class Document:IComparable<Document>
    {
        private readonly string _document;

        public Document(string document)
        {
            _document = document;
        }

        public int CompareTo(Document other) => string.Compare(_document, other._document, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => _document;
    }

    public class DivisionName:IComparable<DivisionName>
    {
        private readonly string _divisionName;

        public DivisionName(string divisionName)
        {
            _divisionName = divisionName;
        }

        public int CompareTo(DivisionName other) => string.Compare(_divisionName, other._divisionName, StringComparison.OrdinalIgnoreCase);

        public override string ToString() => _divisionName;
    }

    public class StaffInfo:IComparable<StaffInfo>
    {
        private readonly FullName _staffFullName;
        private readonly Occupation _occupation;
        private readonly District _district;

        public StaffInfo(FullName name, Occupation occupation, District district)
        {
            (_staffFullName, _occupation, _district) = (name, occupation, district);
        }

        public int CompareTo(StaffInfo other)
        {
            var cmpName = _staffFullName.CompareTo(other._staffFullName);
            if (cmpName != 0) return cmpName;
            var cmpOccupation = _occupation.CompareTo(other._occupation);
            return cmpOccupation != 0 ? cmpOccupation : _district.CompareTo(other._district);
        }

        public override string ToString() => string.Join(" ", _staffFullName, _occupation, _district);

        public FullName GetFullName() => _staffFullName;

        public Occupation GetOccupation() => _occupation;

        public District GetDistrict() => _district;
    }

    public class StaffNameAndOccupation:IComparable<StaffNameAndOccupation>
    {
        private readonly FullName _staffName;
        private readonly Occupation _staffOccupation;

        public StaffNameAndOccupation(FullName name, Occupation occupation)
        {
            (_staffName, _staffOccupation) = (name, occupation);
        }

        public int CompareTo(StaffNameAndOccupation other)
        {
            var cmpName = _staffName.CompareTo(other._staffName);
            return cmpName != 0 ? cmpName : _staffOccupation.CompareTo(other._staffOccupation);
        }

        public override int GetHashCode()
        {
            return _staffName.GetHashCode() + _staffOccupation.GetHashCode();
        }

        public override string ToString() => string.Join(" ", _staffName, _staffOccupation);
    }

    public class WorkPlace:IComparable<WorkPlace>
    {
        private readonly Occupation _occupation;
        private readonly District _district;

        public WorkPlace(Occupation occupation, District district)
        {
            (_occupation, _district) = (occupation, district);
        }

        public int CompareTo(WorkPlace other)
        {
            var cmpOccupation = _occupation.CompareTo(other._occupation);
            return cmpOccupation != 0 ? cmpOccupation : _district.CompareTo(other._district);
        }

        public override string ToString() => string.Join(" ", _occupation, _district);
    }

    public class DocumentInfo:IComparable<DocumentInfo>
    {
        private readonly Document _document;
        private readonly Occupation _occupation;
        private readonly DivisionName _divisionName;

        public DocumentInfo(Document document, Occupation occupation, DivisionName divisionName)
        {
            (_document, _occupation, _divisionName) = (document, occupation, divisionName);
        }

        public int CompareTo(DocumentInfo other)
        {
            var cmpDocument = _document.CompareTo(other._document);
            if (cmpDocument != 0) return cmpDocument;
            var cmpOccupation = _occupation.CompareTo(other._occupation);
            return cmpOccupation != 0 ? cmpOccupation : _divisionName.CompareTo(other._divisionName);
        }

        public override string ToString() => string.Join(" ", _document, _occupation, _divisionName);

        public Document GetDocument() => _document;

        public Occupation GetOccupation() => _occupation;
        
        public DivisionName GetDivisionName() => _divisionName;
    }
}