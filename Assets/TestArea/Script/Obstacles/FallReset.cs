using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallReset : MonoBehaviour
{
    public Transform spawnPoint; // Reference to the spawn point

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player is affected
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // Disable CharacterController to reset position
                other.transform.position = spawnPoint.position; // Move player to spawn point
                controller.enabled = true; // Re-enable CharacterController
            }
        }
    }
}
