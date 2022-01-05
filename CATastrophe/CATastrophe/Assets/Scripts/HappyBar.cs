using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHappy(int happiness)
    {
        slider.maxValue = happiness;
        slider.value = happiness;
    }

    public void SetHappy(int happiness)
    {
        slider.value = happiness;
    }
}
