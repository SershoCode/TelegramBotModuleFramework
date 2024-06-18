using System.Text;

namespace TBMF.Core;

public static class StringExtensions
{
    private static readonly StringBuilder _stringBuilder = new();

    public static string RemoveSpecialCharacters(this string str)
    {
        _stringBuilder.Clear();

        foreach (char c in str)
        {
            if ((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'))
                _stringBuilder.Append(c);
        }

        return _stringBuilder.ToString();
    }
}