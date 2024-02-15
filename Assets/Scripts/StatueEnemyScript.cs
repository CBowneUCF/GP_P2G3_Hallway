using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StatueEnemyScript : MonoBehaviour
{
    //Parameters
    public float viewRadius;
    public float moveSpeed;
    public float attackRadius;

    //public NavMeshObstacle frustrumObstacle;
    
    //Components
    NavMeshAgent agent;
    new Transform transform;
    new Renderer renderer;
    public Transform player;
    public Camera playerCamera;
    public Transform sneakUpTarget;

    //Other Data
    //bool playerLookingInDirection;
    bool canMove;
    //Ray rayToPlayer;
    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        transform = base.transform;
        renderer = GetComponent<Renderer>();
    }


    void Update()
    {
        //rayToPlayer = new Ray(transform.position, (player.position - transform.position).normalized);
        //playerLookingInDirection = Vector3.Dot(rayToPlayer.direction, player.eulerAngles) <= viewRadius;
        //frustrumObstacle.enabled = (Vector3.Dot(rayToPlayer.direction, player.eulerAngles) >= -0.9);


        DetermineMovementCapability();

        SetSneakTargetPosition();
        
        if (!agent.isStopped)
        {
            if (Vector3.Distance(player.position, transform.position) <= attackRadius) agent.destination = player.position;
            else agent.destination = sneakUpTarget.position;
        }



    }

    void DetermineMovementCapability()
    {
        if (!IsPlayerLooking()) ChangeMovementCapability(true);
        else
        {
            if (IsSightBlocked()) ChangeMovementCapability(true);
            else ChangeMovementCapability(false);
        }
    }
    void ChangeMovementCapability(bool value)
    {
        agent.isStopped = !value;
        if(!value) agent.velocity = Vector3.zero;
    }
    void SetSneakTargetPosition()
    {
        Vector3 toPlayerDirection = (player.position - transform.position);

        //1 if same direction, -1 if opposite, 0 if perpendicular
        float DotProd = Vector3.Dot(-toPlayerDirection.normalized, player.forward.normalized);

        float playerStatueDistance = Vector3.Distance(player.position, transform.position);
        float distanceFromPlayer = playerStatueDistance * DotProd;

        Vector3 direction = -player.right * Mathf.Sign(Vector3.Dot(player.right, toPlayerDirection));
        //Vector3 backPadding = -player.forward * Mathf.Abs(Vector3.Dot(player.right, toPlayerDirection));

        sneakUpTarget.position = player.position + direction * distanceFromPlayer;

        //Debug.DrawRay(player.position, -toPlayerDirection);
    }


    //Frustrum Check Code from https://youtu.be/_e57zSZSOS8
    bool IsPlayerLooking() => GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(playerCamera), renderer.bounds);
    bool IsSightBlocked()
    {
        RaycastHit hit;
        Physics.Raycast(
            transform.position, 
            (player.position - transform.position).normalized, 
            out hit, 
            Vector3.Distance(player.position, transform.position));
        return hit.transform != player;
    }



}
