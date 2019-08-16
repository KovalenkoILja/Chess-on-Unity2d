using UnityEngine;

// ReSharper disable once InconsistentNaming
public class FPSDisplay : MonoBehaviour
{
    #region Variables

    public float DeltaTime { get; set; }
    public static bool IsShowed = true;

    #endregion

    #region UnityEvents

    public void Update()
    {
        DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
    }

    public void OnGUI()
    {
        if (!IsShowed) return;

        var millisecond = DeltaTime * 1000.0f;
        var fps = 1.0f / DeltaTime;

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height * 2 / 100.0f),
            $"{millisecond:0.0} ms ({fps:0.} fps)",
            new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = Colors.DebugColor}
            });
    }

    private void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag("FPS Display");

        if (objs.Length > 1) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    #endregion
}