using Game.Entities;
using Game.Tile;
using System.Collections;
using Assets.Code.Code;
using UnityEngine;
using Assets.Code.Assets.Code.Runtime;
using Game.Systems.Map;

public class CameraBehaviour : MonoBehaviour
{
    public float panSpeed = 5f;
    public bool edgeScrolling = false;
    public Camera Camera;
    private static bool lerping = false;
    private void FocusOnTile(TileModel t)
    {
        Debug.Log($"Focusing on tile {t}");
        var coroutine = LerpTo(new Vector3(t.X - 2, 5, t.Y - 2), 0.2f);
        StartCoroutine(coroutine);
    }

    private void Start()
    {
        ClientViewState.OnSelectEntity += OnSelectEntity;
    }

    private void OnSelectEntity(IUnityEntityView e)
    {
        if(e.Entity.Components.TryGet<MapPlacementComponent>(out var placed))
        {
            FocusOnTile(e.Entity.GetTile());
        }
    }

    /*
    private void Update()
    {
        if(ClientState.SelectedEntity != null)
        {
            transform.position =
                Vector3.SmoothDamp(transform.position,
                CameraPosition(ClientState.SelectedEntity.GameObject.transform.position),
                ref velocity, _smoothTime);
        }
    }
    */

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
            ClientViewState.CameraPosition = pos;
        }
    }
}