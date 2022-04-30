#if UNITY_DEBUG
using System;
using System.Collections.Generic;
using UnityEngine;

partial class NanoPool<T>
{
    public readonly List<RetainObject> retainObjects = new List<RetainObject>();
    public bool saveStackTrace;

    private void OnRetain(T obj)
    {
        if (IndexOf(obj) != -1)
        {
            throw _pool.Contains(obj)
                ? new Exception($"Unexpected state! The pool has several objects: [{obj.GetType()}] '{obj}'?")
                : new Exception($"[{obj.GetType()}] '{obj}' object already retained!");
        }

        retainObjects.Add(new RetainObject
        {
            obj = obj,
            frame = Time.frameCount,
            stackTrace = saveStackTrace
                ? Environment.StackTrace
                : string.Empty,
        });
    }

    private void OnRelease(T obj)
    {
        if (IndexOf(obj) == -1)
        {
            throw _pool.Contains(obj)
                ? new Exception($"[{obj.GetType()}] '{obj}' object already released!")
                : new Exception($"[{obj.GetType()}] '{obj}' object was created outside the pool!");
        }

        for (int i = 0, n = retainObjects.Count; i < n; i++)
        {
            if (ReferenceEquals(retainObjects[i].obj, obj))
            {
                retainObjects.RemoveAt(i);
                --i;
                --n;
            }
        }
    }

    private int IndexOf(T obj)
    {
        for (int i = 0, n = retainObjects.Count; i < n; i++)
        {
            if (ReferenceEquals(retainObjects[i].obj, obj))
            {
                return i;
            }
        }
        return -1;
    }

    public struct RetainObject
    {
        public T obj;
        public int frame;
        public string stackTrace;
    }
}
#else
using System.Diagnostics;

partial class NanoPool<T>
{
    [Conditional("UNITY_DEBUG")]
    private void OnRetain(T _)
    {
    }

    [Conditional("UNITY_DEBUG")]
    private void OnRelease(T _)
    {
    }
}
#endif // UNITY_DEBUG