using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Weapons;
    public GameObject MuzzlePoint;

    public Animator Gun;
    public Animator JamJar;

    public AudioController AudioControl;
    public BackgroundMusic BgMusic;

    public HealthBar HealthBar;
    public UltBar UltBar;
}
