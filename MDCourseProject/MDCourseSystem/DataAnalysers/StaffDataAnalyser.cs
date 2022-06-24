using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesStaffAnalyser:DataAnalyser
{
    public AddValuesStaffAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
        
    }
    
    public override bool IsCorrectInputData()
    {
        var noSyntaxErrorInName = _textBoxes[0].Text.Split().Length == 3 && _textBoxes[0].Text != string.Empty 
                                 && _textBoxes[0].Text.Split().All(str => str.Length == 1 || str.Substring(1,str.Length - 1).All(char.IsLower))
                                 && _textBoxes[0].Text.Split().Select(str => str.All(sym => char.IsLetter(sym) || sym == '-') && char.IsUpper(str[0])).All(checkFio => checkFio);
        var noSyntaxErrorInOccupation = _textBoxes[1].Text.All(sym => char.IsLetter(sym) || sym is ' ' or '-') && _textBoxes[1].Text != string.Empty;
        var noSyntaxErrorInDistrict = _textBoxes[2].Text.All(sym => char.IsLetterOrDigit(sym) || sym is '-' or '/' or '.' or ',' or ' ')
                              && _textBoxes[2].Text != string.Empty;
        if (!(noSyntaxErrorInName && noSyntaxErrorInOccupation && noSyntaxErrorInDistrict))
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        return noSyntaxErrorInName && noSyntaxErrorInOccupation && noSyntaxErrorInDistrict;

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