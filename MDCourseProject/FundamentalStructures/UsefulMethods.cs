using System;
using System.Linq;
using MDCourseProject.MDCourseSystem.MDCatalogues;

namespace FundamentalStructures;

public static class UsefulMethods
{
    public static int ConvertStringToNumber(string input)
    {
        int k = 0;
        return 1 + input.ToLowerInvariant().Sum(c => c + k++);
    }

    public static ClientFullNameAndTelephone GetClientFullNameAndTelephoneFromString(string input)
    {
        var fullNameAndPhone = input.Split(',');
        fullNameAndPhone[0] = fullNameAndPhone[0].Trim();
        fullNameAndPhone[1] = fullNameAndPhone[1].Trim();

        var clientFullname = fullNameAndPhone[0].Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < clientFullname.Length; i++)
        {
            clientFullname[i] = clientFullname[i].Trim();
        }
        
        return new ClientFullNameAndTelephone(
            name: clientFullname[0],
            surname: clientFullname[1],
            patronymic: clientFullname[2],
            telephone: fullNameAndPhone[1]
        );
    }
}