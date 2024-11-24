using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip MainBackgroundMusic;
    public AudioClip MainBackgroundMusic2;
    public AudioClip MainBackgroundMusic3;
    public AudioClip MirrorBackgroundMusic;
    public AudioClip MirrorBackgroundMusic2;
    public bool Mirror = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = MainBackgroundMusic;
        audioSource.Play();
    }



    public void MainMusic()
    {
        Console.WriteLine("ere");
        if (Mirror == false)
        {

            int randomIndex1 = UnityEngine.Random.Range(0, 2);
            switch (randomIndex1)
            {
                case 0:
                    audioSource.clip = MirrorBackgroundMusic;
                    break;
                case 1:
                    audioSource.clip = MirrorBackgroundMusic2;
                    break;
            }
            audioSource.Play();
            Mirror = true;
        }
        else
        {

            int randomIndex = UnityEngine.Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0:
                    audioSource.clip = MainBackgroundMusic;
                    break;
                case 1:
                    audioSource.clip = MainBackgroundMusic2;
                    break;
                case 2:
                    audioSource.clip = MainBackgroundMusic3;
                    break;
            }

            audioSource.Play();
            Mirror = false;
        }
    }


}
