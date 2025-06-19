using UnityEngine;

/// <summary>
/// Camera movement script for third person games.
/// Attach this script to an empty object, and make the MainCamera a child of it.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;
    [Space]
    public float sensitivity = 5f;
    public Vector2 cameraLimit = new Vector2(-45, 40);

    [Tooltip("Offset from the player: X=left/right, Y=height, Z=backward")]
    public Vector3 cameraOffset = new Vector3(0, 5, -8);

    float mouseX;
    float mouseY;

    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        if (!clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (clickToMoveCamera && Input.GetAxisRaw("Fire2") == 0)
            return;

        // Mouse movement
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Rotate around player
        Quaternion rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        transform.rotation = rotation;

        // Update position with offset
        Vector3 targetPosition = player.position + rotation * cameraOffset;
        transform.position = targetPosition;

        // Zoom with scroll wheel
        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
            cameraOffset.z += zoomAmount;
            cameraOffset.z = Mathf.Clamp(cameraOffset.z, -15, -3); // Clamp zoom range
        }
    }
}