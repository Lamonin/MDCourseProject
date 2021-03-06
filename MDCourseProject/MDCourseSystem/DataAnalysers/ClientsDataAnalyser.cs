using System.Windows;
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
        return true;
    }
}

public class AddValuesApplicationAnalyser:DataAnalyser
{
    public AddValuesApplicationAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }

    public override bool IsCorrectInputData()
    {
        bool isError = false;
        foreach (var textbox in _textBoxes)
        {
            if (textbox.Text.Trim().Length == 0)
            {
                isError = true;
                break;
            }
        }
        
        isError = isError || !MDSystem.clientsSubsystem._clients.ClientsTable.ContainsKey(new ClientFullNameAndTelephone(_textBoxes[4].Text, _textBoxes[5].Text, _textBoxes[6].Text, _textBoxes[7].Text));
        FullName name = new FullName(_textBoxes[0].Text+" "+_textBoxes[1].Text+" "+_textBoxes[2].Text);
        Occupation occupation = new Occupation(_textBoxes[3].Text);
        isError = isError && !MDSystem.staffSubsystem.StaffCatalogue.StaffTable.ContainsKey(new StaffNameAndOccupation(name,occupation));
        
        if (isError)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        return !isError;
    }
}

public class ReportClientsAnalyser : DataAnalyser
{
    public ReportClientsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    public override bool IsCorrectInputData()
    {
        return true;
    }
}