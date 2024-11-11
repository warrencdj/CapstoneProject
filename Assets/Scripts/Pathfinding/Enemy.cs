using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    //Patrolling
    Vector3 destPoint;
    bool walkPointSet;
    [SerializeField] float range;

    //State Change
    [SerializeField] float sightRange;
    bool playerInSight;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        // Check if player is in sight range
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if (playerInSight)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Patrol()
    {
        if (!walkPointSet)
        {
            SearchForDest();
        }
        if (walkPointSet)
        {
            agent.SetDestination(destPoint);
        }
        if (Vector3.Distance(transform.position, destPoint) < 10)
        {
            walkPointSet = false;
        }
    }

    void SearchForDest()
    {
        float z = Random.Range(-range, range);
        float x = Random.Range(-range, range);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkPointSet = true;
        }
    }

}
