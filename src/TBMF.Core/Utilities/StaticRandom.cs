namespace TBMF.Core;

public static class StaticRandom
{
    private static readonly ThreadLocal<Random> ThreadLocal = new(() => new Random(Interlocked.Increment(ref _seed)));
    public static Random Instance => ThreadLocal.Value;

    private static int _seed;

    static StaticRandom()
    {
        _seed = Environment.TickCount;
    }
}