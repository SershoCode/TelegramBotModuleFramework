namespace TBMF.Core;

public static class Periodic
{
    public static string Minutely() => "* * * * *";

    public static string Hourly(int minute) => $"{minute} * * * *";

    public static string Daily(int hour, int minute) => $"{minute} {hour} * * *";

    public static string Weekly(DayOfWeek dayOfWeek, int hour, int minute) => $"{minute} {hour} * * {(int)dayOfWeek}";
}