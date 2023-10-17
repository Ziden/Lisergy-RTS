using System;
using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Tile;
using System.Collections;
using Assets.Code.Code;
using UnityEngine;


public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour Get()
    {
        return _instance;
    }

    private static CameraBehaviour _instance;
    public float panSpeed = 5f;
    public float rotSpeed = 10f;
    public float zoomSpeed = 50f;
    public float borderWidth = 10f;
    public bool edgeScrolling = false;
    public Camera Camera;
    private float zoomMin = -100;
    private float zoomMax = 200;
    private float mouseX, mouseY;

    private static int _focusSpeed = 30;
    private TileEntity Focus = null;
    private static bool lerping = false;

    public static void FocusOnTile(TileEntity t)
    {
        Log.Debug($"Focusing on tile {t}");

        var camera = Get();
        var coroutine = camera.LerpTo(new Vector3(t.X - 2, 5, t.Y - 2), 0.2f);
        camera.StartCoroutine(coroutine);
    }

    IEnumerator LerpTo(Vector3 pos2, float duration)
    {
        var pos1 = Get().transform.position;
        lerping = true;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }

        lerping = false;
        transform.position = pos2;
    }


    private void Start()
    {
        _instance = this;
        Debug.Log("Camera Initialized");
    }

    public void MoveCamera(Vector2 velocity)
    {
        if (lerping || velocity == Vector2.zero) return;
        // Local variable to hold the camera target's position during each frame
        Vector3 pos = transform.position;
        var old = pos;
        velocity = velocity.Rotate(315);
        pos += new Vector3(velocity.x,0,velocity.y) * panSpeed * Time.deltaTime;
        if (old != pos)
        {
            transform.position = pos;
            UIEvents.CameraMoved(pos);
        }
    }
}