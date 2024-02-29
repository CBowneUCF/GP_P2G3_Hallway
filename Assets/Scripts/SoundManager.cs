using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] sounds;

    private AudioSource walkSound;
    private AudioSource dieSound;

    void Start()
    {
        sounds = GetComponents<AudioSource>();

        walkSound = sounds[0];
        dieSound = sounds[1];
    }

    public void PlayWalkSound()
    {
        walkSound.Play();
    }
}
