using Xunit.Abstractions;

namespace TBMF.Core.Tests;

public class NumberExtensionsTests(ITestOutputHelper outputHelper)
{
    private readonly ITestOutputHelper _outputHelper = outputHelper;

    [Fact]
    public void Humanize_Digits_Returns_Correct()
    {
        const int oneBillion = 1_000_000_000;
        const int oneHundredMillions = 100_000_000;
        const int tenMillions = 10_000_000;
        const int oneMillion = 1_000_000;
        const int oneHundredThousand = 100_000;
        const int tenThousand = 10_000;
        const int oneThousand = 1_000;
        const int oneHundred = 100;

        var oneBillionHumanized = oneBillion.Shortinize();
        var oneHundredMillionsHumanized = oneHundredMillions.Shortinize();
        var tenMillionsHumanized = tenMillions.Shortinize();
        var oneMillionHumanized = oneMillion.Shortinize();
        var oneHundredThousandHumanized = oneHundredThousand.Shortinize();
        var tenThousandHumanized = tenThousand.Shortinize();
        var oneThousandHumanized = oneThousand.Shortinize();
        var oneHundredHumanized = oneHundred.Shortinize();

        _outputHelper.WriteLine($"1B: {oneBillionHumanized}");
        _outputHelper.WriteLine($"100M: {oneHundredMillionsHumanized}");
        _outputHelper.WriteLine($"10M: {tenMillionsHumanized}");
        _outputHelper.WriteLine($"1M: {oneMillionHumanized}");
        _outputHelper.WriteLine($"100K: {oneHundredThousandHumanized}");
        _outputHelper.WriteLine($"10K: {tenThousandHumanized}");
        _outputHelper.WriteLine($"1K: {oneThousandHumanized}");
        _outputHelper.WriteLine($"100: {oneHundredHumanized}");

        Assert.Equal("1B", oneBillionHumanized);
        Assert.Equal("100M", oneHundredMillionsHumanized);
        Assert.Equal("10M", tenMillionsHumanized);
        Assert.Equal("1M", oneMillionHumanized);
        Assert.Equal("100K", oneHundredThousandHumanized);
        Assert.Equal("10K", tenThousandHumanized);
        Assert.Equal("1K", oneThousandHumanized);
        Assert.Equal("100", oneHundredHumanized);
    }

    [Theory]
    [InlineData("Сообщение")]
    [InlineData("Шортс")]
    [InlineData("Голосовое")]
    [InlineData("Картинка")]
    [InlineData("Мат")]
    [InlineData("Стикер")]
    [InlineData("Войс")]
    public void Quantity_Returns_Correct_String(string word)
    {
        _outputHelper.WriteLine(new string('-', 20));

        const int maxCounter = 200;

        for (int i = 1; i < maxCounter; i++)
        {
            _outputHelper.WriteLine(i.ToRussianQuantity(word));
        }

        _outputHelper.WriteLine(new string('-', 20));
    }
}