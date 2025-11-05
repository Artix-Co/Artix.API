namespace Artix.API.Utils.List;



public static class ListExtensions
{
    private static readonly Random _random = new();

    public static void ShuffleInPlace<T>(this List<T> list, Random? rng = null)
    {
        rng ??= _random;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
