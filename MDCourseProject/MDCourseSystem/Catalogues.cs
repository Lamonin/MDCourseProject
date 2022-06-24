using System.Collections.Generic;

namespace MDCourseProject.MDCourseSystem;

interface ICatalogue
{
    public void SetCatalogue(int index);
    public List<string> catalogues { get; }
}
    
class ClientsCatalogue:ICatalogue
{
    public ClientsCatalogue()
    {
        MDSystem.currentCatalogue = CatalogueTypeEnum.Clients;
    }

    public void SetCatalogue(int index)
    {
        if (index == 0) //Первый каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.Clients;
        }
        else //Второй каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.Appeals;
        }
    }

    public List<string> catalogues => new(){"Клиенты", "Обращения"};
}
    
class StaffCatalogue:ICatalogue
{
    public StaffCatalogue()
    {
        MDSystem.currentCatalogue = CatalogueTypeEnum.Staff;
    }

    public void SetCatalogue(int index)
    {
        if (index == 0) //Первый каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.Staff;
        }
        else //Второй каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.Documents;
        }
    }

    public List<string> catalogues => new(){"Сотрудники", "Документы"};
}

class DivisionsCatalogue:ICatalogue
{
    public DivisionsCatalogue()
    {
        MDSystem.currentCatalogue = CatalogueTypeEnum.Divisions;
    }

    public void SetCatalogue(int index)
    {
        if (index == 0) //Первый каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.Divisions;
        }
        else //Второй каталог
        {
            MDSystem.currentCatalogue = CatalogueTypeEnum.SendRequests;
        }
    }

    public List<string> catalogues => new(){"Подразделения", "Отправленные заявки"};
}