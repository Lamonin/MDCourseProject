using System.Windows;
using System.Windows.Controls;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesDivisionAnalyser: DataAnalyser
{
    public AddValuesDivisionAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    
    public override bool IsCorrectInputData()
    {
        //ЛОГИКА АНАЛИЗАТОРА
        return true;
    }
    
}

public class AddValuesSendRequestsAnalyser: DataAnalyser
{
    public AddValuesSendRequestsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }

    public override bool IsCorrectInputData()
    {
        //ЛОГИКА АНАЛИЗАТОРА
        bool isHasErrorInInputData = false;

        if (_textBoxes[0].Text.Length < 3)
            isHasErrorInInputData = true;
        
        if (_textBoxes[1].Text.Length < 2)
            isHasErrorInInputData = true;

        if (isHasErrorInInputData)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        return !isHasErrorInInputData;
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