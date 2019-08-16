using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    #region Variables

    public CursorMode cursorMode = CursorMode.Auto;
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    #endregion

    #region UnityEvents

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void Awake()
    {
        var obj = GameObject.FindGameObjectsWithTag("Cursor");

        if (obj.Length > 1) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    #endregion
}