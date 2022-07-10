using System;
using FundamentalStructures;

namespace MDCourseProject.MDCourseSystem.MDCatalogues;

public class SendRequest : IComparable<SendRequest>
{
    public readonly DivisionNameAndArea Division;

    public SendRequest(DivisionNameAndArea division, string client, string service, string date)
    {
        Division = division;
        DivisionName = Division.ToString();
        Client = client;
        Service = service;
        Date = date;
    }

    public int CompareTo(SendRequest other)
    {
        var compareRes = Division.CompareTo(other.Division);
        if (compareRes != 0) return compareRes;

        var client = UsefulMethods.GetClientFullNameAndTelephoneFromString(Client);
        var otherClient = UsefulMethods.GetClientFullNameAndTelephoneFromString(other.Client);

        compareRes = client.CompareTo(otherClient);
        if (compareRes != 0) return compareRes;

        compareRes = string.Compare(Service, other.Service, StringComparison.OrdinalIgnoreCase);
        if (compareRes != 0) return compareRes;

        return string.Compare(Date, other.Date, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return string.Join(", ", DivisionName, Client, Service, Date);
    }

    public string DivisionName { get; set; }
    public string Client { get; set; }
    public string Service { get; set; }
    public string Date { get; set; }
}

public readonly struct DivisionNameAndArea : IComparable<DivisionNameAndArea>
{
    public DivisionNameAndArea(string name, string area)
    {
        Name = name;
        Area = area;
    }

    public int CompareTo(DivisionNameAndArea other)
    {
        var nameComparison = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        if (nameComparison != 0) return nameComparison;

        return string.Compare(Area, other.Area, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return UsefulMethods.ConvertStringToNumber(Name) + UsefulMethods.ConvertStringToNumber(Area);
    }

    public override string ToString()
    {
        return Name + " (" + Area + ")";
    }

    public readonly string Name;
    public readonly string Area;
}

public struct Division : IComparable<Division>
{
    public Division(string name, string area, string type)
    {
        Name = name;
        Area = area;
        Type = type;
    }

    public int CompareTo(Division other)
    {
        var compareRes = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        if (compareRes != 0) return compareRes;

        compareRes = string.Compare(Area, other.Area, StringComparison.OrdinalIgnoreCase);
        if (compareRes != 0) return compareRes;

        return string.Compare(Type, other.Type, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return $"{Name} ({Area})";
    }

    public string Name { get; set; }
    public string Area { get; set; }
    public string Type { get; set; }
}