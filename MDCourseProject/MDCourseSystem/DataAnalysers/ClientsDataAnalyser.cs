using System.Windows.Controls;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.MDCourseSystem.MDCatalogues;

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

public class AddValuesApplicationAnalyser:DataAnalyser
{
    public AddValuesApplicationAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
    
    public override bool IsCorrectInputData()
    {
        foreach (var textbox in _textBoxes)
        {
            if (textbox.Text.Trim().Length == 0) return false;
        }
        
        var client = MDSystem.clientsSubsystem._clients.ClientsTable.ContainsKey(new ClientFullNameAndTelephone(_textBoxes[4].Text, _textBoxes[5].Text, _textBoxes[6].Text, _textBoxes[7].Text));
        return client;
    }
}