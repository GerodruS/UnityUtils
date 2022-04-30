using System.Collections.Generic;

public partial class NanoPool<T>
    where T : class, new()
{
    public static readonly NanoPool<T> instance = new NanoPool<T>();

    private readonly Stack<T> _pool = new Stack<T>();

    public T Retain()
    {
        T result = Get();
        OnRetain(result);
        return result;
    }

    public void Release(ref T obj)
    {
        OnRelease(obj);
        _pool.Push(obj);
        obj = null;
    }

    private T Get()
    {
        T result = _pool.Count != 0
            ? _pool.Pop()
            : new T();
        return result;
    }
}