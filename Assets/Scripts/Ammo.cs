using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    public int maxAmmo;
    public int ammo;
    public TextMeshProUGUI ammoDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ammoDisplay.SetText(ammo.ToString()+"/"+maxAmmo.ToString());
    }
}
