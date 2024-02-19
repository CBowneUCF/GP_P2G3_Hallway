using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardItemScript : InteractableScript
{
    public float speed;
    public PlayerScript player;

    new Transform transform;
    Vector3 deSpawnLocation;
    
    private void Start()
    {
        transform = GetComponent<Transform>();
        deSpawnLocation = player.transform.position;
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.up * speed;
    }

    public override void Interact()
    {
        player.transform.position = deSpawnLocation;
    }
}
