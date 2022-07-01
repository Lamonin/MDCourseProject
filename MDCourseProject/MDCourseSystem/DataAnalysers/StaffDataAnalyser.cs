using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.AppWindows.DataAnalysers;


public interface ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes);

    public bool CheckInOtherCatalogue(TextBox[] textBoxes, out string[] text);
}

public class CheckStaffCatalogue: ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes)
    {
        var noSyntaxErrorInName = textBoxes[0].Text.Split().Length == 3 && textBoxes[0].Text != string.Empty 
                                    && textBoxes[0].Text.Split().All(str => str.Length == 1 || str.Substring(1,str.Length - 1).All(char.IsLower))
                                    && textBoxes[0].Text.Split().Select(str => str.All(sym => char.IsLetter(sym) || sym == '-') && char.IsUpper(str[0])).All(checkFio => checkFio);
        var noSyntaxErrorInOccupation = textBoxes[1].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && textBoxes[1].Text != string.Empty;
        var noSyntaxErrorInDistrict = textBoxes[2].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or '/' or '.' or ',' or ' ')
                                      && textBoxes[2].Text != string.Empty;
        return noSyntaxErrorInName && noSyntaxErrorInOccupation && noSyntaxErrorInDistrict;
    }

    public bool CheckInOtherCatalogue(TextBox[] textBoxes, out string[] text)
    {
        text = Array.Empty<string>();
        return true;
    }
}

public class CheckDocumentCatalogue: ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes)
    {
        var noSyntaxErrorInDocument = textBoxes[0].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or ' ')
                                      && textBoxes[0].Text != string.Empty;
        var noSyntaxErrorInOccupation = textBoxes[1].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && textBoxes[1].Text != string.Empty;
        var noSyntaxErrorInDivision = textBoxes[2].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && textBoxes[2].Text != string.Empty;
        return noSyntaxErrorInDocument && noSyntaxErrorInOccupation && noSyntaxErrorInDivision;
    }

    public bool CheckInOtherCatalogue(TextBox[] textBoxes, out string[] text)
    {
        text = new string[2];
        var checkInStaff = MDSystem.staffSubsystem.StaffCatalogue.OccupationTree.ContainKey(new Occupation(textBoxes[1].Text));
        if (!checkInStaff)
            text[0] = $"Такой должности {textBoxes[1].Text} нет в справочнике <{MDSystem.staffSubsystem.StaffCatalogue.Name}>";
        var checkInDivision = MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsByName.Contains(textBoxes[2].Text);
        if (!checkInDivision)
            text[1] = $"Такого подразделения {textBoxes[2].Text} нет в справочнике <{MDSystem.divisionsSubsystem.DivisionsCatalogue.Name}>";
        return checkInDivision && checkInStaff;
    }
}

public class ReportCheck: ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes)
    {
        var noSyntaxErrorInDocument = textBoxes[0].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or ' ')
                                      && textBoxes[0].Text != string.Empty;
        var noSyntaxErrorInDistrict = textBoxes[1].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or '/' or '.' or ',' or ' ')
                                      && textBoxes[1].Text != string.Empty;
        return noSyntaxErrorInDocument && noSyntaxErrorInDistrict;
    }

    public bool CheckInOtherCatalogue(TextBox[] textBoxes, out string[] text)
    {
        text = Array.Empty<string>();
        return true;
    }
}

public class SearchCheckInDocument: ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes)
    {
        var noSyntaxErrorInDocument = textBoxes[0].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or ' ')
                                      && textBoxes[0].Text != string.Empty;
        return noSyntaxErrorInDocument;
    }

    public bool CheckInOtherCatalogue(TextBox[] textBoxes, out string[] text)
    {
        text = Array.Empty<string>();
        return true;
    }
}
public static class CheckCorrectnessOfData
{
    public static bool Check(ICheck checkSystem, TextBox[] textBoxes)
    {
        var checkSyntax = checkSystem.CheckSyntax(textBoxes);
        if(!checkSyntax)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        var checkExistence = checkSystem.CheckInOtherCatalogue(textBoxes, out var error);
        if(!checkExistence)
        {
            if (error[0] != null)
                MessageBox.Show(error[0], "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            if(error[1] != null)
                MessageBox.Show(error[1], "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        return true;
    }
}

public class AddValuesStaffAnalyser:DataAnalyser
{
    public AddValuesStaffAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
        
    }
    
    public override bool IsCorrectInputData()
    {
        return CheckCorrectnessOfData.Check(new CheckStaffCatalogue(), _textBoxes);
    }
}

public class AddValuesDocumentsAnalyser:DataAnalyser
{
    public AddValuesDocumentsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        return CheckCorrectnessOfData.Check(new CheckDocumentCatalogue(), _textBoxes);
    }
}

public class RemoveValuesDocumentAnalyser: DataAnalyser
{
    public RemoveValuesDocumentAnalyser(TextBox[] textBoxes): base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        return CheckCorrectnessOfData.Check(new CheckDocumentCatalogue(), _textBoxes);
    }
}

public class SearchValuesDocumentAnalyser: DataAnalyser
{
    public SearchValuesDocumentAnalyser(TextBox[] textBoxes): base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        return CheckCorrectnessOfData.Check(new SearchCheckInDocument(), _textBoxes);
    }
}

public class ReportStaffSubSystemAnalyser: DataAnalyser
{
    public ReportStaffSubSystemAnalyser(TextBox[] textBoxes): base(textBoxes)
    {
    }

    public override bool IsCorrectInputData()
    {
        return CheckCorrectnessOfData.Check(new ReportCheck(), _textBoxes);
    }
}