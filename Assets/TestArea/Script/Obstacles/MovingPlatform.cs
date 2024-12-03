using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f; // Speed of the platform
    private Vector3 direction = Vector3.right; // Initial direction

    private void Update()
    {
        // Move the platform in the current direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for collision with wall
        if (other.CompareTag("Wall"))
        {
            // Reverse direction when hitting a wall
            direction = -direction;
        }
    }
}