public static class ContainersExtensions
{
    public static int TryCount<T>(this T[] array)
    {
        return array?.Length ?? 0;
    }
}