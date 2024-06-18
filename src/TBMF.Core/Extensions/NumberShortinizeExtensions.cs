using System.Globalization;

namespace TBMF.Core;

public static class NumberShortinizeExtensions
{
    private static readonly NumberFormatInfo NumberFormatInfo = new() { CurrencyDecimalSeparator = "." };

    public static string Shortinize(this int num) => ShortinizeNumber(num);

    public static string Shortinize(this long num) => ShortinizeNumber(num);

    public static string Shortinize(this double num) => ShortinizeNumber(Convert.ToInt64(num));

    private static string ShortinizeNumber(long num)
    {
        if (num >= 1000000000)
        {
            return GetNumberWithDotSeparator(num / 1000000000D, "0.###B");
        }

        if (num >= 100000000)
        {
            return GetNumberWithDotSeparator(num / 1000000D, "0.##M");
        }

        if (num >= 1000000)
        {
            return GetNumberWithDotSeparator(num / 1000000D, "0.##M");
        }

        if (num >= 100000)
        {
            return GetNumberWithDotSeparator(num / 1000D, "0.##K");
        }

        if (num >= 10000)
        {
            return GetNumberWithDotSeparator(num / 1000D, "0.##K");
        }

        if (num >= 1000)
        {
            return GetNumberWithDotSeparator(num / 1000D, "0.##K");
        }

        return num.ToString("#,0");
    }

    private static string GetNumberWithDotSeparator(double number, string pattern)
    {
        return number.ToString(pattern, NumberFormatInfo);
    }
}