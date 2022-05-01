using UnityEngine;
using UnityObject = UnityEngine.Object;

public static class ObjectExtensions
{
    public static string GetPath(object simpleObject, bool addInstanceId = false, bool addGlobalObjectId = false)
    {
#if UNITY_DEBUG
        return simpleObject is UnityObject unityObject
            ? unityObject.GetPath(addInstanceId, addGlobalObjectId)
            : simpleObject.ToString();
#else
        return string.Empty;
#endif
    }

    public static string GetPath(this UnityObject unityObject, bool addInstanceId = false, bool addGlobalObjectId = false)
    {
#if UNITY_DEBUG
        var gameObject = unityObject as GameObject;
        if (gameObject != null)
        {
            var path = gameObject.transform.GetPath();
            if (addGlobalObjectId)
            {
    #if UNITY_EDITOR
                var globalObjectId = UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(gameObject);
                path += $" ({globalObjectId.targetObjectId})";
    #endif
            }
            if (addInstanceId)
            {
                path += $" [{gameObject.GetInstanceID()}]";
            }
            return path;
        }

        var transform = unityObject as Transform;
        if (transform != null)
        {
            return transform.GetPath(addInstanceId, addGlobalObjectId);
        }

        var component = unityObject as Component;
        if (component != null)
        {
            var path = component.transform.GetPath();
            if (addGlobalObjectId)
            {
    #if UNITY_EDITOR
                var globalObjectId = UnityEditor.GlobalObjectId.GetGlobalObjectIdSlow(component);
                path += $" ({globalObjectId.targetObjectId})";
    #endif
            }
            if (addInstanceId)
            {
                path += $" [{component.GetInstanceID()}]";
            }
            return path;
        }
#endif // UNITY_DEBUG

        return unityObject.name;
    }
}