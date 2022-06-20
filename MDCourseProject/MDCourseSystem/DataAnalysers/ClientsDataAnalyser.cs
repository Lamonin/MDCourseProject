using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesClientsAnalyser:DataAnalyser
{
    public AddValuesClientsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        return false;
    }
}

public class AddValuesAppealsAnalyser:DataAnalyser
{
    public AddValuesAppealsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        return false;
    }
}