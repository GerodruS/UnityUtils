using System.Collections.Generic;
using UnityEngine;

public static class ContainersExtensions
{
    public static int TryCount<T>(this T[] array)
    {
        return array?.Length ?? 0;
    }

    public static void UpdateCapacity<T>(this List<T> list, int capacity)
    {
        if (list.Capacity < capacity)
        {
            capacity = Mathf.Max(capacity, 4);
            capacity = capacity.Pow2RoundUp();
            list.Capacity = capacity;
        }
    }
}