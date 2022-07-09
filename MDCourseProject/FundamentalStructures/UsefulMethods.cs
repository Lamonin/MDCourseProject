using System.Linq;

namespace FundamentalStructures;

public static class UsefulMethods
{
    public static int ConvertStringToNumber(string input)
    {
        int k = 0;
        return 1 + input.ToLowerInvariant().Sum(c => c + k++);
    }
}