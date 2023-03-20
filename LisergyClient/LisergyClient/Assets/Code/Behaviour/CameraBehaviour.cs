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
    public Camera cam;
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
        var coroutine = camera.LerpFromTo(camera.transform.position, new Vector3(t.X - 2, 5, t.Y - 2), 0.2f);
        camera.StartCoroutine(coroutine);
    }

    private void OnEnable()
    {
        Global.InputManager().OnCameraMove += OnCameraMovement;
    }

    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
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
    }

    void OnCameraMovement(Vector2 velocity)

    {
        if (lerping) return;
        // Local variable to hold the camera target's position during each frame
        Vector3 pos = transform.position;
        var old = pos;
        velocity = velocity.Rotate(315);
        pos += new Vector3(velocity.x,0,velocity.y) * panSpeed * Time.deltaTime;
        if (old != pos)
        {
            transform.position = pos;
            ClientEvents.CameraMove(transform.position, pos);
        }
    }


    void Rotation()

    {
        // If Mouse Button 1 is pressed, (the secondary (usually right) mouse button)

        if (Input.GetMouseButton(1))

        {
            // Our mouseX variable gets set to the X position of the mouse multiplied by the rotation speed added to it.

            mouseX += Input.GetAxis("Mouse X") * rotSpeed;

            // Our mouseX variable gets set to the Y position of the mouse multiplied by the rotation speed added to it.

            mouseY -= Input.GetAxis("Mouse Y") * rotSpeed;

            // Clamp the minimum and maximum angle of how far the camera can look up and down.

            mouseY = Mathf.Clamp(mouseY, -30, 45);

            // Set the rotation of the camera target along the X axis (pitch) to mouseY (up & down) & Y axis (yaw) to mouseX (left & right), the Z axis (roll) is always set to 0 as we do not want the camera to roll.

            transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
    }


    void Zoom()

    {
        // Local variable to temporarily store our camera's position

        Vector3 camPos = cam.transform.position;

        // Local variable to store the distance of the camera from the camera_target

        float distance = Vector3.Distance(transform.position, cam.transform.position);


        // When we scroll our mouse wheel up, zoom in if the camera is not within the minimum distance (set by our zoomMin variable)

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && distance > zoomMin)

        {
            camPos += cam.transform.forward * zoomSpeed * Time.deltaTime;
        }


        // When we scroll our mouse wheel down, zoom out if the camera is not outside of the maximum distance (set by our zoomMax variable)

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && distance < zoomMax)

        {
            camPos -= cam.transform.forward * zoomSpeed * Time.deltaTime;
        }


        // Set the camera's position to the position of the temporary variable

        cam.transform.position = camPos;
    }

    // End of file
}