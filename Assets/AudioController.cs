using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioController : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip flip;
    public AudioClip FireMainwep;
    // Start is called before the first frame update
    void Start()
    {
     audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void FlipToMirror()
    {
        audioSource.clip = flip;
        audioSource.Play();
    }

    public void FireMain()
    {
        audioSource.clip = FireMainwep;
        audioSource.Play();
        
    }
}
