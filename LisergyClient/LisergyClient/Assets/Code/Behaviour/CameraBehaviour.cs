using Assets.Code;
using Assets.Code.World;
using Game;
using System.Collections;
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
    private ClientTile Focus = null;
    private static bool lerping = false;

    public static void FocusOnTile(ClientTile t)
    {
        Log.Debug($"Focusing on tile {t}");

        var camera = Get();
        var coroutine = camera.LerpFromTo(camera.transform.position, new Vector3(t.X - 2, 5, t.Y - 2), 0.2f);
        camera.StartCoroutine(coroutine);
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

    void Update()

    {
        if(!lerping)
            Movement();
    }

    private void Start()
    {
        _instance = this;
    }

    void Movement()

    {

        // Local variable to hold the camera target's position during each frame

        Vector3 pos = transform.position;

        // Local variable to reference the direction the camera is facing (Which is driven by the Camera target's rotation)

        Vector3 forward = transform.forward;

        // Ensure the camera target doesn't move up and down

        forward.y = 0;

        // Normalize the X, Y & Z properties of the forward vector to ensure they are between 0 & 1

        forward.Normalize();


        // Local variable to reference the direction the camera is facing + 90 clockwise degrees (Which is driven by the Camera target's rotation)

        Vector3 right = transform.right;

        // Ensure the camera target doesn't move up and down

        right.y = 0;

        // Normalize the X, Y & Z properties of the right vector to ensure they are between 0 & 1

        right.Normalize();


        // Move the camera (camera_target) Forward relative to current rotation if "W" is pressed or if the mouse moves within the borderWidth distance from the top edge of the screen

        if (Input.GetKey("w") || edgeScrolling == true && Input.mousePosition.y >= Screen.height - borderWidth)

        {

            pos += forward * panSpeed * Time.deltaTime;

        }


        // Move the camera (camera_target) Backward relative to current rotation if "S" is pressed or if the mouse moves within the borderWidth distance from the bottom edge of the screen

        if (Input.GetKey("s") || (edgeScrolling == true && Input.mousePosition.y <= borderWidth))

        {

            pos -= forward * panSpeed * Time.deltaTime;

        }


        // Move the camera (camera_target) Right relative to current rotation if "D" is pressed or if the mouse moves within the borderWidth distance from the right edge of the screen

        if (Input.GetKey("d") || (edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth))

        {

            pos += right * panSpeed * Time.deltaTime;

        }


        // Move the camera (camera_target) Left relative to current rotation if "A" is pressed or if the mouse moves within the borderWidth distance from the left edge of the screen

        if (Input.GetKey("a") || (edgeScrolling == true && Input.mousePosition.x <= borderWidth))

        {

            pos -= right * panSpeed * Time.deltaTime;

        }


        // Setting the camera target's position to the modified pos variable
        var old = transform.position;
        if(old != pos)
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

