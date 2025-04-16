using System.Text.RegularExpressions;

namespace Retro95;

public class NaturalSort : IComparer<string?>
{
    public int Compare(string? x, string? y)
    {
        switch (x, y)
        {
            case (null, null):
                return 0;
            case (not null, null):
                return -1;
            case (null, not null):
                return 1;
        }

        var xParts = Regex.Split(x, "(\\d+)");
        var yParts = Regex.Split(y, "(\\d+)");

        foreach (var (xPart, yPart) in xParts.Zip(yParts))
        {
            int cmp;
            switch (int.TryParse(xPart, out var xAsInt), int.TryParse(yPart, out var yAsInt))
            {
                case (true, true):
                    cmp = xAsInt.CompareTo(yAsInt);
                    break;
                case (true, false):
                    return -1;
                case (false, true):
                    return 1;
                case (false, false):
                    cmp = string.Compare(xPart, yPart, StringComparison.InvariantCulture);
                    break;
            }
            
            if (cmp != 0)
            {
                return cmp;
            }
        }

        return xParts.Length.CompareTo(yParts.Length);
    }
}