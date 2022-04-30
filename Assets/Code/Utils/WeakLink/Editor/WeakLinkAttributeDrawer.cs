using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[CustomPropertyDrawer(typeof(WeakLinkAttribute))]
internal class WeakLinkAttributeDrawer : PropertyDrawer
{
    #region PropertyDrawer
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (attribute is not WeakLinkAttribute weakLinkAttribute) return;

        var guid = property.stringValue;
        var found = GuidToUnityObject(guid, weakLinkAttribute.type, out var unityObject);
        if (!found && !string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning($"{weakLinkAttribute.type.Name} '{guid}' not found!");
        }

        EditorGUI.BeginProperty(position, label, property);
        var pickedUnityObject = EditorGUI.ObjectField(position, label, unityObject, weakLinkAttribute.type, false);
        EditorGUI.EndProperty();

        if (UnityObjectToGuid(pickedUnityObject, out var pickedGuid) &&
            (pickedUnityObject != weakLinkAttribute.cachedObject ||
             pickedGuid != guid))
        {
            property.stringValue = pickedGuid;
            property.serializedObject.ApplyModifiedProperties();
            weakLinkAttribute.cachedObject = pickedUnityObject;
        }

        if (pickedUnityObject == null &&
            (weakLinkAttribute.cachedObject != null ||
             !string.IsNullOrEmpty(guid)))
        {
            property.stringValue = string.Empty;
            weakLinkAttribute.cachedObject = null;
        }
    }
    #endregion PropertyDrawer

    private static bool GuidToUnityObject(string guid, Type type, out UnityObject unityObject)
    {
        if (string.IsNullOrEmpty(guid))
        {
            unityObject = default;
            return false;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(assetPath))
        {
            unityObject = default;
            return false;
        }

        unityObject = AssetDatabase.LoadAssetAtPath(assetPath, type);
        return unityObject != null;
    }

    private static bool UnityObjectToGuid(UnityObject unityObject, out string guid)
    {
        if (unityObject == null)
        {
            guid = default;
            return false;
        }

        var assetPath = AssetDatabase.GetAssetPath(unityObject);
        if (string.IsNullOrEmpty(assetPath))
        {
            guid = default;
            return false;
        }

        guid = AssetDatabase.AssetPathToGUID(assetPath);
        return !string.IsNullOrEmpty(guid);
    }
}