using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using Vector3Helper;

public class StatueEnemyScript : MonoBehaviour
{
    //Parameters
    //public float viewRadius;
    public float moveSpeed;
    public float attackRadius;
    public LayerMask sightLayerMask;
    public float sightPadding;
    public float killRadius;
    
    //Components
    NavMeshAgent agent;
    new Transform transform;
    private new Renderer renderer;
    Transform rendererTransform;

    public Transform player;
    public Camera playerCamera;
    public Transform sneakUpTarget;
    public BoxCollider renderBounds;


    //Other Data
    //bool playerLookingInDirection;
    bool canMove;
    //Ray rayToPlayer;
    private LayerMask sightLayerMask_Old;
    private float statueRadius_Old;
    //Plane[] viewPlanes;
    //[SerializeField] float dDistance;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        transform = base.transform;
        renderer = GetComponentInChildren<Renderer>();
        rendererTransform = renderBounds.transform;
    }


    void Update()
    {
        //viewPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        DetermineMovementCapability();

        //SetSneakTargetPosition();
        
        if (!agent.isStopped)
        {
            agent.destination = player.position;
            /*
            if (Vector3.Distance(player.position, transform.position) <= attackRadius) agent.destination = player.position;
            else agent.destination = sneakUpTarget.position;
             */
        }

        //dDistance = Vector3.Distance(transform.position, player.position);
        Debug.DrawRay(transform.position, (player.position-transform.position).normalized * killRadius, Color.red);
        if (canMove && (Vector3.Distance(transform.position, player.position) <= killRadius)) Debug.Log("SNAP");

    }

    void DetermineMovementCapability()
    {
        if (!IsPlayerLooking()) ChangeMovementCapability(true);
        else
        {
            if (IsSightBlockedV3()) ChangeMovementCapability(true);
            else ChangeMovementCapability(false);
        }
        

    }
    void ChangeMovementCapability(bool value)
    {
        canMove = value;
        agent.isStopped = !value;
        if (!value)
        {
            agent.velocity = Vector3.zero;
            agent.destination = agent.nextPosition;
        }
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
    bool IsSightBlockedV1()
    {
        float midDistance = Vector3.Distance(transform.position, player.position);
        Vector3 midDirection = (transform.position - player.position).normalized;
        RaycastHit hit;
        Physics.Raycast(player.position, midDirection, out hit, midDistance, sightLayerMask_Old);

        bool midHit = (hit.transform == transform);

        float angle = Mathf.Rad2Deg*Mathf.Atan(statueRadius_Old/midDistance);

        Vector3 leftDirection = Quaternion.AngleAxis(-angle, Vector3.up) * midDirection;
        Vector3 rightDirection = Quaternion.AngleAxis(angle, Vector3.up) * midDirection; ;

        float sideDistance = Mathf.Sqrt(Mathf.Pow(midDistance, 2) + Mathf.Pow(statueRadius_Old, 2));

        Physics.Raycast(player.position, leftDirection, out hit, sideDistance, sightLayerMask_Old);
        bool leftHit = (hit.transform == transform);
        Physics.Raycast(player.position, rightDirection, out hit, sideDistance, sightLayerMask_Old);
        bool rightHit = (hit.transform == transform);

        Debug.DrawRay(player.position, midDirection * midDistance, midHit? Color.white:Color.red);
        Debug.DrawRay(player.position, leftDirection * sideDistance, leftHit ? Color.white:Color.red);
        Debug.DrawRay(player.position, rightDirection * sideDistance, rightHit ? Color.white:Color.red);
        

        return !(midHit || rightHit || leftHit);
    }
    
    bool IsSightBlockedV2()
    {
        Position playerPos = player.transform.position;
        Position enemyPosCenter = transform.position;

        Direction enemyToPlayerDirection = new Direction(playerPos - enemyPosCenter).normalized;
        Direction enemyToPlayerPerpendicular = enemyToPlayerDirection.Rotate(90, Direction.up);

        bool midHit = SeeRay(enemyPosCenter);
        bool leftHit = SeeRay(enemyPosCenter - (enemyToPlayerPerpendicular * statueRadius_Old));
        bool rightHit = SeeRay(enemyPosCenter + (enemyToPlayerPerpendicular * statueRadius_Old));

        return !(midHit || rightHit || leftHit);
    }

    bool IsSightBlockedV3()
    {
        //Bounds rendererBounds = renderer.bounds;
        Position centerPos = rendererTransform.TransformPoint(renderBounds.center);

        Position forwardRightPos = (Vector3)centerPos + (rendererTransform.forward * (renderBounds.size.z/2 + sightPadding))    + (rendererTransform.right * (renderBounds.size.x/2 + sightPadding));
        Position forwardLeftPos =  (Vector3)centerPos + (rendererTransform.forward * (renderBounds.size.z/2 + sightPadding))    - (rendererTransform.right * (renderBounds.size.x/2 + sightPadding));
        Position backRightPos =    (Vector3)centerPos - (rendererTransform.forward * (renderBounds.size.z/2 + sightPadding))    + (rendererTransform.right * (renderBounds.size.x/2 + sightPadding));
        Position backLeftPos =     (Vector3)centerPos - (rendererTransform.forward * (renderBounds.size.z / 2 + sightPadding))  - (rendererTransform.right * (renderBounds.size.x/2 + sightPadding));


        bool originHit = SeeRay(transform.position);
        bool midHit = SeeRay(centerPos);
        bool forwardRightHit = SeeRay(forwardRightPos);
        bool backRightHit = SeeRay(backRightPos);
        bool forwardLeftHit = SeeRay(forwardLeftPos);
        bool backLeftHit = SeeRay(backLeftPos);


        return !(originHit || midHit || forwardLeftHit || forwardRightHit || backLeftHit || backRightHit);
    }

    bool SeeRay(Position start)
    {
        Vector3 withinCam = playerCamera.WorldToViewportPoint(start);
        if (!(withinCam.z > 0 && withinCam.x < 1 && withinCam.x > 0 && withinCam.y < 1 && withinCam.y > 0)) return false;

        RaycastHit hit;
        Physics.Linecast(start, player.transform.position, out hit, sightLayerMask);
        bool result = hit.transform == player.transform;

        Debug.DrawLine(start, player.transform.position, result ? Color.white : Color.red);
        return result;
    }




}
