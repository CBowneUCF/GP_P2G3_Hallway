using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwap : MonoBehaviour
{
    public void WinScreen()
    {
        SceneManager.LoadScene("Win Screen");
    }

    public void LoseScreen()
    {
        SceneManager.LoadScene("Lose Screen");
    }
}

