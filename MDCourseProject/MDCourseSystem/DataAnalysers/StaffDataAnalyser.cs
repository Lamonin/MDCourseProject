using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;


public interface ICheck
{
    public bool CheckSyntax(TextBox[] textBoxes);
    
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
        if (!(noSyntaxErrorInName && noSyntaxErrorInOccupation && noSyntaxErrorInDistrict))
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        return noSyntaxErrorInName && noSyntaxErrorInOccupation && noSyntaxErrorInDistrict;
    }
}

public class AddValuesStaffAnalyser:DataAnalyser
{
    public AddValuesStaffAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
        
    }
    
    public override bool IsCorrectInputData()
    {
        return new CheckStaffCatalogue().CheckSyntax(_textBoxes);
    }
}

public class RemoveValuesStaffAnalyser: DataAnalyser
{
    public RemoveValuesStaffAnalyser(TextBox[] textBoxes): base(textBoxes){}

    public override bool IsCorrectInputData()
    {
        return false;
    }
}

public class SearchValuesStaffAnalyser: DataAnalyser
{
    public SearchValuesStaffAnalyser(TextBox[] textBoxes): base(textBoxes){}

    public override bool IsCorrectInputData()
    {
        return false;
    }
}

public class AddValuesDocumentsAnalyser:DataAnalyser
{
    public AddValuesDocumentsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        var noSyntaxErrorInDocument = _textBoxes[0].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or ' ')
                                      && _textBoxes[0].Text != string.Empty;
        var noSyntaxErrorInOccupation = _textBoxes[1].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && _textBoxes[1].Text != string.Empty;
        var noSyntaxErrorInDivision = _textBoxes[2].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && _textBoxes[2].Text != string.Empty;
        if (!(noSyntaxErrorInDocument && noSyntaxErrorInOccupation && noSyntaxErrorInDivision))
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        return noSyntaxErrorInDocument && noSyntaxErrorInOccupation && noSyntaxErrorInDivision;
    }
}