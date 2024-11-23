using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform player;  // Reference to the player object

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(5f, 2f, -10f);  // Offset distance between the player and camera

    [Header("Camera Movement")]
    public float smoothSpeed = 0.125f;  // Smoothness of the camera's movement
    public float lookAheadDistance = 2f;  // Distance ahead of the player for the camera to look

    [Header("Clamping Settings (Optional)")]
    public bool enableClamping = false;  // Enable or disable clamping
    public float minX = 0f;  // Minimum x position of the camera
    public float maxX = 50f; // Maximum x position of the camera
    public float minY = -5f; // Minimum y position of the camera
    public float maxY = 5f;  // Maximum y position of the camera

    void LateUpdate()
    {
        if (player != null)  // Ensure the player exists
        {
            // Calculate the desired position
            Vector3 desiredPosition = new Vector3(
                player.position.x + offset.x + lookAheadDistance,
                player.position.y + offset.y,
                offset.z
            );

            // Smoothly interpolate between the current position and the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // If clamping is enabled, restrict the camera's position within the specified bounds
            if (enableClamping)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
            }

            // Set the camera's position
            transform.position = smoothedPosition;
        }
    }
}