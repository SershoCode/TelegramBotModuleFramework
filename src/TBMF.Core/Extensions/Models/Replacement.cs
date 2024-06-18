namespace TBMF.Core;

internal class Replacement : QuantityBase
{
    public string WordEnding { get; set; }
    public string MinNumbersReplace { get; set; }
    public string MidNumbersReplace { get; set; }
    public string MaxNumbersReplace { get; set; }

    private const int LenghtOfTwoDigitNumber = 2;

    public string GetQuantity(int num, string word)
    {
        var index = word.Length - WordEnding.Length;

        var lastDigit = num % 100; // 7, 11, 98, 99, 

        var firstDigitInNumber = Math.DivRem(lastDigit, 10, out int secondDigitInNumber);

        // Проверить, является ли остаток от числа двузначным.
        var isLastDigitAreTwoDigitNumber = IsLastDigitAreTwoDigitNumber(lastDigit);

        if (isLastDigitAreTwoDigitNumber)
        {
            // Найти 11, 12, 13... 19.
            if (MinNumbers.Contains(firstDigitInNumber) && MaxNumbersReplace != "")
                return Replace(num, word, index, MaxNumbersReplace);

            // Найти 21, 31, 41... 91.
            if (MinNumbers.Contains(secondDigitInNumber) && MinNumbersReplace != "")
                return Replace(num, word, index, MinNumbersReplace);

            // Найти 22, 33, 44... 94.
            if (MidNumbers.Contains(secondDigitInNumber) && MidNumbersReplace != "")
                return Replace(num, word, index, MidNumbersReplace);
        }

        // Найти 1.
        if (MinNumbers.Contains(secondDigitInNumber) && MinNumbersReplace != "")
            return Replace(num, word, index, MinNumbersReplace);

        // Найти 2, 3, 4.
        if (MidNumbers.Contains(secondDigitInNumber) && MidNumbersReplace != "")
            return Replace(num, word, index, MidNumbersReplace);

        // Найти 0, 5, 6, 7, 8, 9.
        if (MaxNumbers.Contains(secondDigitInNumber) && MaxNumbersReplace != "")
            return Replace(num, word, index, MaxNumbersReplace);

        return $"{num} {word}";
    }

    private static bool IsLastDigitAreTwoDigitNumber(int lastDigit)
    {
        var lenghtOfLastDigit = lastDigit.ToString().Length;

        return lenghtOfLastDigit == LenghtOfTwoDigitNumber;
    }

    private static string Replace(int num, string word, int index, string value)
    {
        return $"{num} {word.Remove(index).Insert(index, value)}";
    }
}