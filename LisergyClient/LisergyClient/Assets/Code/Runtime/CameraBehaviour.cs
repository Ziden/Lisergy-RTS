using System;
using Assets.Code;
using Assets.Code.World;
using Game;
using Game.Tile;
using System.Collections;
using Assets.Code.Code;
using UnityEngine;
using Game.ECS;

public class CameraBehaviour : MonoBehaviour
{
    public float panSpeed = 5f;
    public bool edgeScrolling = false;
    public Camera Camera;
    private static bool lerping = false;

    private void FocusOnTile(TileEntity t)
    {
        Debug.Log($"Focusing on tile {t}");
        var coroutine = LerpTo(new Vector3(t.X - 2, 5, t.Y - 2), 0.2f);
        StartCoroutine(coroutine);
    }

    private void Start()
    {
        UIEvents.OnSelectEntity += OnSelectEntity;
    }

    private void OnSelectEntity(IEntity e)
    {
        if(e is BaseEntity be && be.Tile != null)
        {
            FocusOnTile(be.Tile);
        }
    }

    IEnumerator LerpTo(Vector3 pos2, float duration)
    {
        var pos1 = transform.position;
        lerping = true;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        lerping = false;
        transform.position = pos2;
    }

    public void MoveCamera(Vector2 velocity)
    {
        if (lerping || velocity == Vector2.zero) return;
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