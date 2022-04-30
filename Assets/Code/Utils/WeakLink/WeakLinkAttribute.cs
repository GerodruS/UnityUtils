using System;
using System.Diagnostics;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public class WeakLinkAttribute : PropertyAttribute
{
    public readonly Type type;
    public UnityObject cachedObject;

    public WeakLinkAttribute(Type type)
    {
        this.type = type;
    }
}
