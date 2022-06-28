using System.Windows;
using System.Windows.Controls;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.MDCourseSystem.MDSubsystems;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesDivisionAnalyser: DataAnalyser
{
    public AddValuesDivisionAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    
    public override bool IsCorrectInputData()
    {
        bool isError = _textBoxes[0].Text.Length < 2; //Длина названия подразделения меньше двух
        isError = isError || _textBoxes[1].Text.Length < 2; //Длина названия района меньше двух
        
        if (isError)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        return !isError;
    }
    
}

public class AddValuesSendRequestsAnalyser: DataAnalyser
{
    public AddValuesSendRequestsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }

    public override bool IsCorrectInputData()
    {
        //ЛОГИКА АНАЛИЗАТОРА
        bool isError = !MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsTable.ContainsKey(new DivisionNameAndArea(_textBoxes[1].Text, _textBoxes[0].Text));
        
        if (isError)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        return !isError;
    }
}

public class RemoveValuesDivisionsAnalyser: DataAnalyser
{
    public RemoveValuesDivisionsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
}

public class RemoveValuesSendRequestsAnalyser : DataAnalyser
{
    public RemoveValuesSendRequestsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
}

public class SearchValuesDivisionsAnalyser : DataAnalyser
{
    public SearchValuesDivisionsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
}

public class SearchValuesSendRequestsAnalyser : DataAnalyser
{
    public SearchValuesSendRequestsAnalyser(TextBox[] textBoxes) : base(textBoxes)
    {
    }
}

public class ReportDivisionsAnalyser : DataAnalyser
{
    public ReportDivisionsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    public override bool IsCorrectInputData()
    {
        bool isError = !MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsByService.Contains(_textBoxes[0].Text);

        if (isError)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        return !isError;
    }
}

public class ReportSendRequestsAnalyser : DataAnalyser
{
    public ReportSendRequestsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
}