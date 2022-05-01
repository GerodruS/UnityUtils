using System;
using UnityEngine;

public static class TransformExtensions
{
    public static string GetPath(
        this Transform transform,
        bool addInstanceId = false,
        bool addGlobalObjectId = false)
    {
#if UNITY_DEBUG
        var path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        path = transform.gameObject.scene.name + "/" + path;

    #if UNITY_EDITOR
        if (addGlobalObjectId)
        {
            var globalObjectId = UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(transform);
            path += $" ({globalObjectId.targetObjectId})";
        }
    #endif

        if (addInstanceId)
        {
            path += $" [{transform.GetInstanceID()}]";
        }
        return path;
#else
        return transform.name;
#endif
    }

    public static void CheckMissingPrefabs(this Transform transform)
    {
#if UNITY_DEBUG
        var text = string.Empty;
        CheckMissingPrefabs(transform, ref text);
        if (0 < text.Length)
        {
            throw new NullReferenceException($"Missing Prefabs: {text}");
        }
#endif
    }

    public static void CheckMissingPrefabs(this Transform transform, ref string text)
    {
#if UNITY_DEBUG
        if (transform.name.EndsWith("(Missing Prefab)", StringComparison.Ordinal))
        {
            text += $"\n# '{transform.GetPath()}'";
            return;
        }

    #if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabAssetType(transform) == UnityEditor.PrefabAssetType.MissingAsset)
        {
            // looks like it's a prefab variant, that has lost its origin
            text += $"\n# '{transform.GetPath()}'";
            return;
        }
    #endif

        foreach (Transform child in transform)
        {
            CheckMissingPrefabs(child, ref text);
        }
#endif
    }

#if UNITY_EDITOR
    public static Transform GetChild(this Transform transform, string name)
    {
        foreach (Transform child in transform)
        {
            if (string.Equals(child.name, name, StringComparison.Ordinal))
            {
                return child;
            }
        }
        return null;
    }
#endif
}