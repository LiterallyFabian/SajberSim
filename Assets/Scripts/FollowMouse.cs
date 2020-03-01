using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;
    public Texture2D cursorTexture;
    private Vector2 cursorHotspot;
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.ForceSoftware);
    }
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3 (mousePosition.x, mousePosition.y-0.07f, -1);
    }
}
