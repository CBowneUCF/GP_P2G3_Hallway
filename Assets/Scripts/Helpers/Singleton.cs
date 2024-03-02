using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Class is a Base Template for the Singleton Design pattern.
//Any Class that inherets from this will be a Singleton.
//Singletons make it impossible for a script to exist twice.
//Useful for managers that multiples of can cause weirdness.

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T Instance;
    public static T instance { get { 
            if (Instance == null)
            {
                T findAttempt =  Object.FindFirstObjectByType<T>();
                if (findAttempt != null)
                {
                    findAttempt.Awake();
                    return findAttempt;
                } 
                else
                {
                    Debug.LogWarning("There's no Singleton of that type in this scene.");
                    return null;
                }
            }
            else return Instance; } }

    public static bool IsInitialized
    {
        get { return Instance != null; }
    }

    /// <summary>
    /// This is the Unity Function which runs some code necessary for Singleton Function. Use OnAwake() instead.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Something or someone is attempting to create a second " + 
                typeof(T).ToString() +
                ". Which is a Singleton. If you wish to reset the " +
                typeof(T).ToString() +
                ", destroy the first before instantiating its replacement. The duplicate " +
                typeof(T).ToString() +
                " will now be Deleted."
                );

            Destroy(this);
        }
        else
        {
            Instance = (T) this;
            OnAwake();
            //Debug.Log(
            //    "The " +
            //    typeof(T).ToString() +
            //    " Singleton has been successfully Created/Reset."
            //    );
        }
    }

    protected virtual void OnAwake(){}

    /// <summary>
    /// This is the Unity Function which runs some code necessary for Singleton Function. Use OnDestroyed() instead.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        OnDestroyed();
    }
    protected virtual void OnDestroyed() {}

    /// <summary>
    /// Very Dangerous. Do not use if you don't know what you're doing.
    /// </summary>
    public void Reset()
    {
        GameObject obj = Instance.gameObject;
        Destroy(this);
        obj.AddComponent<T>().Awake();
    }

}
