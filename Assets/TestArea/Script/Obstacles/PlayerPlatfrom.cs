using UnityEngine;

public class PlayerPlatfrom : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";  // Tag to identify the player
    [SerializeField] Transform platform;  // The platform to which the player is attached
    private Vector3 lastPlatformPosition;  // To track the platform's last position when the player detaches
    private bool isOnPlatform = false;  // To check if the player is on the platform

    private void Update()
    {
        // If the player is on the platform, update the last platform position
        if (isOnPlatform)
        {
            lastPlatformPosition = platform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            // Attach the player to the platform
            other.transform.SetParent(platform);
            isOnPlatform = true;

            // Optional: Reset the player's velocity if they have a Rigidbody
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero; // Stop any existing velocity when they land on the platform
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            // Detach the player from the platform
            other.transform.SetParent(null);
            isOnPlatform = false;

            // Optional: Smoothly transfer the player's velocity to avoid teleporting
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Keep the player's velocity based on the platform's movement
                Vector3 platformVelocity = platform.position - lastPlatformPosition;
                playerRb.velocity += platformVelocity / Time.deltaTime;
            }
        }
    }
}
