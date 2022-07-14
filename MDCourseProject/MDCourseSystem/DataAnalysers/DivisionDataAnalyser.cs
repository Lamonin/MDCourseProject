using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MDCourseProject.MDCourseSystem;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace MDCourseProject.AppWindows.DataAnalysers;

public class AddValuesDivisionAnalyser: DataAnalyser
{
    public AddValuesDivisionAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    
    public override bool IsCorrectInputData()
    {
        //Избавляемся от строк состоящих из пробелов
        foreach (var t in _textBoxes) t.Text = t.Text.Trim();
        
        bool isError = _textBoxes[0].Text.Length < 2; //Длина названия подразделения меньше двух
        isError = isError || _textBoxes[1].Text.Length < 2; //Длина названия района меньше двух
        isError = isError || _textBoxes[2].Text.Length < 2; //Длина тип подразделения меньше двух

        if (MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsTable.ContainsKey(
                new DivisionNameAndArea(
                    name: _textBoxes[0].Text,
                    area: _textBoxes[1].Text)
                )
        )
        {
            MessageBox.Show("В районе уже есть подразделение с таким названием!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
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
        //Избавляемся от строк состоящих из пробелов
        foreach (var t in _textBoxes) t.Text = t.Text.Trim();
        
        bool isError = !MDSystem.divisionsSubsystem.DivisionsCatalogue.DivisionsTable.ContainsKey(new DivisionNameAndArea(_textBoxes[1].Text, _textBoxes[0].Text));

        //Проверяем, что длина введенных данных не меньше минимума
        isError = isError ||_textBoxes[0].Text.Length < 2;
        isError = isError || _textBoxes[1].Text.Length < 2;
        isError = isError || _textBoxes[2].Text.Length < 2;
        isError = isError || _textBoxes[3].Text.Length < 2;

        //Проверяем на корректность данные о клиенте
        var clientData = _textBoxes[2].Text.Split(new []{","}, StringSplitOptions.RemoveEmptyEntries);
        isError = isError || clientData.Length!=2; //Данные клиента должны быть разделены запятой не меньше и не больше одного раза
        isError = isError || clientData[0].Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries).Length!=3; //ФИО должно состоять из трёх полей - фамилии, имени и отчества
        isError = isError || clientData[1].Trim().Length != 11; //Кол-во символов номера не должно быть меньше, или больше 11
        isError = isError || !clientData[1].Trim().All(char.IsDigit); //Номер телефона должен состоять только из цифр

        //Проверка корректности даты
        if (DateTime.TryParse(_textBoxes[4].Text, out var time))
            isError = isError || time > DateTime.Today; //Заявка не может быть отправлена позже текущей даты
        else
            isError = true;
        
        //Проверка на существования такого клиента
        var fullName = _textBoxes[2].Text.Split(',')[0].Split();
        if (!MDSystem.clientsSubsystem._clients.ClientsTable.ContainsKey(
                new ClientFullNameAndTelephone(
                    name: fullName[1],
                    surname: fullName[0],
                    patronymic: fullName[2], 
                    telephone: _textBoxes[2].Text.Split(',')[1].Trim()
                )
            )
        ) {
            MessageBox.Show("Такого клиента нет в справочнике Клиенты!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        
        //Проверка на существование такой записи
        var divisionNameAndArea = new DivisionNameAndArea(_textBoxes[1].Text, _textBoxes[0].Text);
        if (MDSystem.divisionsSubsystem.SendRequestsCatalogue.SendRequestsTree.Contains(
                key: divisionNameAndArea,
                val: new SendRequest(
                    division: divisionNameAndArea,
                    client: _textBoxes[2].Text,
                    service: _textBoxes[3].Text,
                    date: _textBoxes[4].Text
                )
            )
        ) {
            MessageBox.Show("Такая заявка уже есть в справочнике!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (isError)
        {
            MessageBox.Show("Некорректные данные!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        return !isError;
    }
}

public class ReportDivisionsAnalyser : DataAnalyser
{
    public ReportDivisionsAnalyser(TextBox[] textBoxes) : base(textBoxes) { }
    public override bool IsCorrectInputData()
    {
        //Избавляемся от строк состоящих из пробелов
        foreach (var t in _textBoxes) t.Text = t.Text.Trim();
        
        //Проверяем введенную дату на корректность
        bool isError = !DateTime.TryParse(_textBoxes[2].Text, out var time1);
        isError |= !DateTime.TryParse(_textBoxes[3].Text, out var time2);
        isError = isError || time1.CompareTo(time2) > 0;

        if (isError)
        {
            MessageBox.Show("Некорректная дата!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        return !isError;
    }
}