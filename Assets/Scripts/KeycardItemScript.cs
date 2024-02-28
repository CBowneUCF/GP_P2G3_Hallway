using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardItemScript : InteractableScript
{
    public float speed;
    public Transform player;

    new Transform transform;
    Vector3 deSpawnLocation;

    //SceneSwap changeScene;
    
    private void Start()
    {
        transform = GetComponent<Transform>();
        deSpawnLocation = player.position;
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.up * speed;
    }

    public override void Interact()
    {
        player.position = deSpawnLocation;
        //changeScene.WinScreen();
    }
}
