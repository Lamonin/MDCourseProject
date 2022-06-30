using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class DataAnalyser
{
    protected readonly TextBox[] _textBoxes;
    
    public DataAnalyser(TextBox[] textBoxes)
    {
        _textBoxes = textBoxes;
    }

    public virtual string[] GetData()
    {
        var data = new string[_textBoxes.Length];

        for (int i = 0; i < _textBoxes.Length; i++)
        {
            data[i] = _textBoxes[i].Text;
        }
        
        return data;
    }
    
    public virtual bool IsCorrectInputData()
    {
        return false;
    }
}