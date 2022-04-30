#if UNITY_DEBUG // DEFINE_EXCEPTION_SCREEN
using UnityEngine;

internal static class ExceptionScreenSpawner
{
    private static bool _isFirstException = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoad()
    {
        Application.LogCallback callback = OnLogMessageReceivedThreaded;
        Application.logMessageReceivedThreaded -= callback;
        Application.logMessageReceivedThreaded += callback;
    }

    private static void OnLogMessageReceivedThreaded(string condition, string stacktrace, LogType type)
    {
        if (type != LogType.Exception) return;

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

        if (!_isFirstException) return;
        _isFirstException = false;

        var gameObject = new GameObject("ExceptionScreen");
        Object.DontDestroyOnLoad(gameObject);

        var exceptionScreen = gameObject.AddComponent<ExceptionScreen>();
        exceptionScreen.text = $"{condition}\n{stacktrace}";

#if UNITY_EDITOR
        Debug.Break();
#endif
    }
}
#endif // UNITY_DEBUG // DEFINE_EXCEPTION_SCREEN