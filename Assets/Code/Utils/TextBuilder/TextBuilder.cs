using System;
using UnityEngine;

public class TextBuilder
{
    private const int START_LENGTH = 32;
    private readonly float[] FLOAT_POWER =
    {
        0.5f,
        0.05f,
        0.005f,
        0.0005f,
        5E-05f,
        5E-06f,
        5E-07f,
        5E-08f,
        5E-09f,
        5E-10f,
    };

    public int length;
    public char[] buffer = new char[START_LENGTH];

    public char this[int i]
    {
        get => buffer[i];
        set => buffer[i] = value;
    }

    public TextBuilder Reset()
    {
        length = 0;
        return this;
    }

    #region Append
    public TextBuilder Append(string value)
    {
        return Append(value, 0, value.Length);
    }

    public TextBuilder Append(string value, int startIndex, int valueLength)
    {
        if (string.IsNullOrEmpty(value)) return this;

        startIndex = Mathf.Max(startIndex, 0);
        valueLength = Mathf.Min(valueLength, value.Length - startIndex);

        if (valueLength <= 0) return this;

        var offset = length;
        Resize(length + valueLength);
        for (int i = 0; i < valueLength; ++i)
        {
            buffer[offset + i] = value[startIndex + i];
        }

        return this;
    }

    public TextBuilder Append(TextBuilder value)
    {
        var offset = length;
        var valueLength = value.length;
        Resize(length + valueLength);
        for (int i = 0; i < valueLength; ++i)
        {
            buffer[offset + i] = value[i];
        }

        return this;
    }

    public TextBuilder Append(char value)
    {
        Resize(length + 1);
        buffer[length - 1] = value;
        return this;
    }

    public TextBuilder Append(int value)
    {
        return Append((long)value);
    }

    public TextBuilder Append(uint value)
    {
        return Append((ulong)value);
    }

    public TextBuilder Append(long value)
    {
        if (value < 0)
        {
            Append('-');
            value = -value;
        }

        Append((ulong)value);
        return this;
    }

    public TextBuilder Append(ulong value)
    {
        if (0 == value)
        {
            Append('0');
        }
        else
        {
            var valueLength = GetLength(value);

            var oldLength = length;
            Resize(length + valueLength);

            for (int i = length - 1; oldLength <= i; --i)
            {
                buffer[i] = (char)('0' + (value % 10));
                value /= 10;
            }
        }

        return this;
    }

    public TextBuilder Append(float value, int precision)
    {
        if (value < 0.0f)
        {
            Append('-');
            value = -value;
        }

        value += FLOAT_POWER[Mathf.Min(FLOAT_POWER.Length - 1, precision)];
        var intValue = (uint)value;
        Append(intValue);
        if (0 < precision)
        {
            Append('.');
            value -= intValue;
            for (int i = 0; i < precision; ++i)
            {
                value *= 10.0f;
            }

            Append((uint)value);
        }

        return this;
    }
    #endregion Append

    #region AppendFormat
    public TextBuilder AppendFormat(string text, TextBuilder anotherStringBuilder)
    {
        var offset = length;
        var textLength = text.Length;
        Resize(length + textLength);
        for (int i = 0; i < textLength; ++i)
        {
            var c = text[i];
            if (c == '{' &&
                i + 2 < textLength &&
                text[i + 1] == '0' &&
                text[i + 2] == '}')
            {
                Resize(offset + i);
                Append(anotherStringBuilder);
                Resize(length + textLength - i - 3);
                i += 2;
            }
            else
            {
                buffer[offset + i] = c;
            }
        }

        return this;
    }
    #endregion AppendFormat

    #region AppendArgument
    public TextBuilder ReplaceArgument(TextBuilder text, int index)
    {
        var oldLength = length;
        var offset = -1;
        for (int i = 0; i < oldLength; ++i)
        {
            if (buffer[i] == '{' &&
                i + 2 < oldLength &&
                buffer[i + 1] == '0' + index &&
                buffer[i + 2] == '}')
            {
                offset = i;
                break;
            }
        }

        if (offset != -1)
        {
            var textLength = text.length;
            var newLength = oldLength - 3 + textLength;
            Resize(newLength);
            Array.Copy(buffer, offset + 3, buffer, offset + textLength, oldLength - offset - 3);
            length = offset;
            Append(text);
            length = newLength;
        }

        return this;
    }

    public TextBuilder ReplaceArgument(string text, int index)
    {
        var changed = true;
        while (changed)
        {
            changed = ReplaceArgumentInternal(text, index);
        }

        return this;
    }

    private bool ReplaceArgumentInternal(string text, int index)
    {
        var oldLength = length;
        var offset = -1;

        for (int i = 0; i < oldLength; ++i)
        {
            if (buffer[i] == '{' &&
                i + 2 < oldLength &&
                buffer[i + 1] == '0' + index &&
                buffer[i + 2] == '}')
            {
                offset = i;
                break;
            }
        }

        if (offset != -1)
        {
            var textLength = text.Length;
            var newLength = oldLength - 3 + textLength;
            Resize(newLength);
            Array.Copy(buffer, offset + 3, buffer, offset + textLength, oldLength - offset - 3);
            length = offset;
            Append(text);
            length = newLength;
            return true;
        }

        return false;
    }

    public TextBuilder ReplaceArgument(int value, int index)
    {
        var oldLength = length;
        var offset = -1;
        for (int i = 0; i < oldLength; ++i)
        {
            if (buffer[i] == '{' &&
                i + 2 < oldLength &&
                buffer[i + 1] == '0' + index &&
                buffer[i + 2] == '}')
            {
                offset = i;
                break;
            }
        }

        if (offset != -1)
        {
            var textLength = GetLength(value);
            var newLength = oldLength - 3 + textLength;
            Resize(newLength);
            Array.Copy(buffer, offset + 3, buffer, offset + textLength, oldLength - offset - 3);
            length = offset;
            Append(value);
            length = newLength;
        }

        return this;
    }

    public TextBuilder ReplaceArgument(float value, int index)
    {
        var oldLength = length;
        var offset = -1;
        var gapLength = 0;
        var precision = 0;
        for (int i = 0; i < oldLength; ++i)
        {
            if (buffer[i] == '{' &&
                i + 2 < oldLength &&
                buffer[i + 1] == '0' + index)
            {
                if (buffer[i + 2] == '}')
                {
                    offset = i;
                    gapLength = 3;
                    break;
                }
                else if (buffer[i + 2] == ':' &&
                         i + 4 < oldLength &&
                         buffer[i + 4] == '}')
                {
                    offset = i;
                    gapLength = 5;
                    precision = buffer[i + 3] - '0';
                    break;
                }
            }
        }

        if (offset != -1)
        {
            var textLength = GetLength(value, precision);
            var newLength = oldLength - gapLength + textLength;
            Resize(newLength);
            Array.Copy(buffer, offset + gapLength, buffer, offset + textLength, oldLength - offset - gapLength);
            length = offset;
            Append(value, precision);
            length = newLength;
        }

        return this;
    }
    #endregion AppendArgument

    #region Object
    public override string ToString()
    {
        return new string(buffer, 0, length);
    }
    #endregion Object

    private void Resize(int requestedLength)
    {
        var newArrayLength = requestedLength.Pow2RoundUp();
        if (buffer.Length < requestedLength)
        {
            Array.Resize(ref buffer, newArrayLength);
        }
        length = requestedLength;
    }

    private static int GetLength(ulong value)
    {
        if (value == 0)
        {
            return 1;
        }
        else
        {
            var length = 0;
            for (; value != 0; value /= 10) ++length;
            return length;
        }
    }

    private static int GetLength(long value)
    {
        return value < 0
            ? GetLength((ulong)-value) + 1
            : GetLength((ulong)value);
    }

    private static int GetLength(float value, int precision)
    {
        if (value < 0)
        {
            return GetLength(-value, precision) + 1;
        }
        else
        {
            var intValue = (uint)value;
            var length = GetLength(intValue);
            return 0 < precision
                ? length + 1 + precision
                : length;
        }
    }
}