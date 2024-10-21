using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float moveSpeed = 5f; // Speed of the enemy
    public float detectionDistance = 10f; // Maximum distance for detection
    public float wanderRadius = 5f; // Radius for wandering
    public float wanderTime = 3f; // Time to wander before choosing a new direction
    private Vector3 startingPosition; // Starting position of the enemy
    private bool playerInSight; // Flag to check if player is in sight
    private float wanderTimer; // Timer for wandering
    private Vector3 wanderTarget; // Target position for wandering

    private void Start()
    {
        // Store the starting position
        startingPosition = transform.position;
        wanderTimer = wanderTime; // Initialize wander timer
        ChooseWanderTarget(); // Choose the initial wander target
    }

    private void Update()
    {
        CheckPlayerVisibility();

        if (playerInSight)
        {
            MoveTowardsPlayer();
        }
        else
        {
            WanderAround();
        }
    }

    private void CheckPlayerVisibility()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Check if the player is within detection distance
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionDistance)
        {
            // Perform a raycast to check for obstacles between the enemy and player
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                // Check if the hit object is the player
                if (hit.transform == player)
                {
                    playerInSight = true; // Player is visible
                    Debug.Log("Player is in sight!"); // Debug log
                }
                else
                {
                    playerInSight = false; // Player is blocked by an obstacle
                    Debug.Log("Player is blocked by an obstacle: " + hit.transform.name); // Debug log
                }
            }
            else
            {
                playerInSight = false; // No hit means player is not visible
            }
        }
        else
        {
            playerInSight = false; // Player is out of detection range
            Debug.Log("Player is out of detection range."); // Debug log
        }
    }

    private void MoveTowardsPlayer()
    {
        // Move the enemy towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

        // Optional: Look at the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void WanderAround()
    {
        // Update the wander timer
        wanderTimer -= Time.deltaTime;

        // If the timer has run out, choose a new wander target
        if (wanderTimer <= 0)
        {
            ChooseWanderTarget();
            wanderTimer = wanderTime; // Reset the timer
        }

        // Move towards the wander target
        Vector3 directionToWanderTarget = (wanderTarget - transform.position).normalized;
        transform.position += directionToWanderTarget * moveSpeed * Time.deltaTime;

        // Optional: Look towards the wander target
        Quaternion targetRotation = Quaternion.LookRotation(directionToWanderTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void ChooseWanderTarget()
    {
        // Choose a random point within the wander radius
        float randomX = Random.Range(-wanderRadius, wanderRadius);
        float randomZ = Random.Range(-wanderRadius, wanderRadius);
        wanderTarget = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    }

    private void ReturnToStartingPoint()
    {
        // Move the enemy back to the starting position
        Vector3 directionToStart = (startingPosition - transform.position).normalized;
        transform.position += directionToStart * moveSpeed * Time.deltaTime;

        // Optional: Look towards the starting position
        Quaternion targetRotation = Quaternion.LookRotation(directionToStart);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        // Draw raycasts in the Scene view for debugging
        Gizmos.color = Color.red;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Gizmos.DrawRay(transform.position, directionToPlayer * detectionDistance);
    }
}