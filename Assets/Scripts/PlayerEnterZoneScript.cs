using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEnterZoneScript : MonoBehaviour
{
    public UnityEvent EnterEvent;
    public bool OneTime = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            EnterEvent?.Invoke();
            if (OneTime) enabled = false;
        }
    }
}
