#if UNITY_DEBUG // DEFINE_EXCEPTION_SCREEN
using JetBrains.Annotations;
using UnityEngine;

internal class ExceptionScreen : MonoBehaviour
{
    [Multiline]
    public string text;

    private GUIStyle _style;
    private GUIStyle Style
    {
        get
        {
            _style ??= new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal =
                {
                    textColor = Color.white,
                    background = Background(),
                },
                wordWrap = true,
                fontStyle = FontStyle.Bold,
            };
            _style.normal.background = _style.normal.background
                ? _style.normal.background
                : Background();
            return _style;

            [NotNull] Texture2D Background()
            {
                var result = new Texture2D(1, 1);
                result.SetPixels(new[] { new Color(0, 0, 0, 0.75f) });
                result.Apply();
                return result;
            }
        }
    }

    #region MonoBehaviour
    private void OnGUI()
    {
        GUI.Box(
            new Rect(0, 0, Screen.width, Screen.height),
            text,
            Style
        );
    }
    #endregion MonoBehaviour
}
#endif // UNITY_DEBUG // DEFINE_EXCEPTION_SCREEN