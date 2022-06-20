using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesStaffAnalyser:DataAnalyser
{
    public AddValuesStaffAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
        
    }
    
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
        return false;
    }
}