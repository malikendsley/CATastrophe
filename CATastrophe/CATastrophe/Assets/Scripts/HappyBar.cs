using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHappy(float happiness)
    {
        slider.maxValue = happiness;
        slider.value = happiness;
    }

    public void SetHappy(float happiness)
    {
        slider.value = happiness;
    }
}
