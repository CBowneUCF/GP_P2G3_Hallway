using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorScript : InteractableScript
{
    //new Collider collider;
    //NavMeshObstacle obstacle;
    Animator animator;

    public bool isClosed = true;
    bool isAnimating = false;

    public string anim_Closed = "Closed";
    public string anim_Open = "Open";
    public string anim_Closing = "Closing";
    public string anim_Opening = "Opening";

    public bool requiresKeycard;
    PlayerScript player;

    private void Start()
    {
        //collider = GetComponent<Collider>();
        //obstacle = GetComponent<NavMeshObstacle>();
        //collider.isTrigger = !isClosed;
        //obstacle.enabled = isClosed;
        animator = GetComponent<Animator>();
        animator.Play(isClosed ? anim_Closed : anim_Open );
        if (requiresKeycard) player = GameStateManagerScript.instance.player;
    }


    public override void Interact()
    {
        if (requiresKeycard) if (!player.hasKeycard) return;

        if (isAnimating) return;

        animator.Play(isClosed ? anim_Opening : anim_Closing);
        isAnimating = true;
        isClosed = !isClosed;

    }

    public void ToggleCollision()
    {
        //collider.isTrigger = !isClosed;
        //obstacle.enabled = isClosed;
    }
    public void EndAnimation() => isAnimating = false;
}
