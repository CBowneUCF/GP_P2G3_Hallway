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
    public LayerMask sightLayerMask;
    public float statueRadius;

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
        Vector3 toPlayerDirection = (player.position - transform.position).normalized;

        //1 if same direction, -1 if opposite, 0 if perpendicular
        float DotProd = Vector3.Dot(-toPlayerDirection, player.forward.normalized);

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
        float midDistance = Vector3.Distance(transform.position, player.position);
        Vector3 midDirection = (transform.position - player.position).normalized;
        RaycastHit hit;
        Physics.Raycast(player.position, midDirection, out hit, midDistance, sightLayerMask);

        bool midHit = (hit.transform == transform);

        float angle = Mathf.Rad2Deg*Mathf.Atan(statueRadius/midDistance);

        Vector3 leftDirection = Quaternion.AngleAxis(-angle, Vector3.up) * midDirection;
        Vector3 rightDirection = Quaternion.AngleAxis(angle, Vector3.up) * midDirection; ;

        float sideDistance = Mathf.Sqrt(Mathf.Pow(midDistance, 2) + Mathf.Pow(statueRadius, 2));

        Physics.Raycast(player.position, leftDirection, out hit, sideDistance, sightLayerMask);
        bool leftHit = (hit.transform == transform);
        Physics.Raycast(player.position, rightDirection, out hit, sideDistance, sightLayerMask);
        bool rightHit = (hit.transform == transform);

        Debug.DrawRay(player.position, midDirection * midDistance, midHit? Color.white:Color.red);
        Debug.DrawRay(player.position, leftDirection * sideDistance, leftHit ? Color.white:Color.red);
        Debug.DrawRay(player.position, rightDirection * sideDistance, rightHit ? Color.white:Color.red);
        

        return !(midHit || rightHit || leftHit);
    }



}
