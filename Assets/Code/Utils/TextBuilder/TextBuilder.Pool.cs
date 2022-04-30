partial class TextBuilder
{
    public static TextBuilder Retain()
    {
        var textBuilder = NanoPool<TextBuilder>.instance.Retain();
        textBuilder.Reset();
        return textBuilder;
    }

    public static void Release(ref TextBuilder textBuilder)
    {
        NanoPool<TextBuilder>.instance.Release(ref textBuilder);
    }
}