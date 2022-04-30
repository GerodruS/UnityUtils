using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AssetsCrawler
{
    #region Component
    #region Prefabs
    public static void CallAllComponentsOfTypeAtPrefabs<T>(Action<T> function, bool save = true, string pathStart = "", string pathEnd = "")
        where T : Component
    {
        CallAllComponentsOfTypeAtPrefabs<T>(
            (_, component) =>
            {
                function(component);
                return save;
            },
            pathStart: pathStart,
            pathEnd: pathEnd);
    }

    public static void CallAllComponentsOfTypeAtPrefabs<T>(Func<string, T, bool> function, string pathStart = "", string pathEnd = "")
        where T : Component
    {
        EditorUtility.DisplayProgressBar("Searching", "", 0f);

        var save = false;
        var paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0, count = paths.Length; i < count; i++)
        {
            var path = paths[i];
            if (!path.EndsWith(".prefab", StringComparison.Ordinal)) continue;
            if (!path.StartsWith(pathStart, StringComparison.Ordinal)) continue;
            if (!path.EndsWith(pathEnd, StringComparison.Ordinal)) continue;

            EditorUtility.DisplayProgressBar($"Calling {typeof(T).Name}", $"{i}/{count} {path}", i / (float)count);

            var prefab = PrefabUtility.LoadPrefabContents(path);
            var components = prefab.GetComponentsInChildren<T>(true);
            var saveAsset = false;
            for (int j = 0, m = components.TryCount(); j < m; j++)
            {
                var saveComponent = function.Invoke(path, components[j]);
                saveAsset = saveAsset || saveComponent;
            }

            if (saveAsset &&
                0 < components.TryCount())
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                save = true;
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        if (save)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        EditorUtility.ClearProgressBar();
    }
    #endregion Prefabs

    #region Scenes
    public static void CallAllComponentsOfTypeAtScenes<T>(Action<T> function, bool save = true, string pathStart = "", string pathEnd = "")
        where T : Component
    {
        CallbackAtAllScenes(() =>
            {
                CallComponentsAtCurrentScene(function);
            },
            save,
            pathStart,
            pathEnd);
    }

    public static void CallAllComponentsOfTypeAtScenes<T1, T2>(Action<T1> f1, Action<T2> f2, bool save = true, string pathStart = "", string pathEnd = "")
        where T1 : Component
        where T2 : Component
    {
        CallbackAtAllScenes(() =>
            {
                CallComponentsAtCurrentScene(f1);
                CallComponentsAtCurrentScene(f2);
            },
            save,
            pathStart,
            pathEnd);
    }

    private static void CallbackAtAllScenes(Action function, bool save = true, string pathStart = "", string pathEnd = "")
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EditorUtility.DisplayProgressBar("Searching", "", 0f);
        var sceneGuids = AssetDatabase.FindAssets("t:Scene");

        for (int i = 0, n = sceneGuids.TryCount(); i < n; i++)
        {
            var sceneGuid = sceneGuids[i];
            var sceneAssetPath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            if (!sceneAssetPath.StartsWith(pathStart)) continue;
            if (!sceneAssetPath.EndsWith(pathEnd)) continue;

            EditorUtility.DisplayProgressBar("Walking through all scenes...", $"{sceneAssetPath} {i}/{n}", i / (float)n);

            EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Single);
            function.Invoke();
            if (save)
            {
                EditorSceneManager.SaveOpenScenes();
            }
        }

        if (save)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorUtility.ClearProgressBar();
    }

    private static void CallComponentsAtCurrentScene<T>(Action<T> function)
        where T : Component
    {
        var components = Object.FindObjectsOfType<T>();
        for (int i = 0, n = components.TryCount(); i < n; i++)
        {
            var component = components[i];
            function.Invoke(component);
            EditorUtility.SetDirty(component);
        }
    }
    #endregion Scenes
    #endregion Component

    #region ScriptableObject
    public static void CallAllScriptableObjectOfType<T>(Action<T> function, bool save = true)
        where T : ScriptableObject
    {
        EditorUtility.DisplayProgressBar("Searching", "", 0f);
        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        var count = guids.TryCount();

        for (var i = 0; i < count; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            var scriptableObject = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            EditorUtility.DisplayProgressBar($"Calling {typeof(T).Name}", $"{scriptableObject.name} {i}/{count}", i / (float)count);
            function.Invoke(scriptableObject);
            if (save)
            {
                EditorUtility.SetDirty(scriptableObject);
            }
        }

        if (save)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorUtility.ClearProgressBar();
    }
    #endregion ScriptableObject
}
