using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    // Start is called before the first frame update


    public void SetHealth(float Health)
    {
        slider.value = Health;
    }

    public void setMaxHealth(float Health)
    {
        slider.maxValue = Health;
        slider.value = Health;

    }

}
