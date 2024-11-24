using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltBar : MonoBehaviour
{
    

    public Slider slider;
    // Start is called before the first frame update


    public void SetUlt(int ult)
    {
        slider.value = ult;
    }

    public void setMaxUlt(int ult)
    {
        slider.maxValue = ult;
        slider.value = ult;

    }
}
