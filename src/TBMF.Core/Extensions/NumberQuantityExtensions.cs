namespace TBMF.Core;

/// <summary>
/// Вызываем на числе .ToRussianQuantity() и передаем туда единственное число слова.
/// Например: 52.ToRussianQuantity("картинка").
/// После обработки получим "52 картинки".
/// </summary>
public static class NumberQuantityExtensions
{
    // MinNumbersReplace - на что меняем, в случае если число оканчивается на 1.
    // MidNumbersReplace - на что меняем, в случае если число оканчивается на 2, 3, 4.
    // MaxNumbersReplace - на что меняем, в случае если число оканчивается на 0, 5, 6, 7, 8, 9.
    private static readonly List<Replacement> Replacements =
    [
        new Replacement { WordEnding = "ие", MinNumbersReplace = "ие", MidNumbersReplace = "ия", MaxNumbersReplace = "ий" },
        new Replacement { WordEnding = "ое", MinNumbersReplace = "ое", MidNumbersReplace = "ых", MaxNumbersReplace = "ых" },
        new Replacement { WordEnding = "ка", MinNumbersReplace = "ка", MidNumbersReplace = "ки", MaxNumbersReplace = "ок" },
        new Replacement { WordEnding = "р", MinNumbersReplace = "р", MidNumbersReplace = "ра", MaxNumbersReplace = "ров" },
        new Replacement { WordEnding = "т", MinNumbersReplace = "т", MidNumbersReplace = "та", MaxNumbersReplace = "тов" },
        new Replacement { WordEnding = "тс", MinNumbersReplace = "тс", MidNumbersReplace = "тса", MaxNumbersReplace = "тсов" },
        new Replacement { WordEnding = "йс", MinNumbersReplace = "йс", MidNumbersReplace = "йса", MaxNumbersReplace = "йсов" },
    ];

    public static string ToRussianQuantity(this int num, string word)
    {
        var replacementOrDefault = Replacements.SingleOrDefault(replacement => word.EndsWith(replacement.WordEnding));

        if (replacementOrDefault is not null)
            return replacementOrDefault.GetQuantity(num, word);

        return $"{num} {word}";
    }
}