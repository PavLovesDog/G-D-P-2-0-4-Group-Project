using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MBF 
{ 
    public class ColdMeter : MonoBehaviour
    {
        public Slider slider;
    
        public void SetMaxCold(float maxCold)
        {
            slider.maxValue = maxCold;
            slider.value = maxCold;
        }
    
        public void SetCurrentCold(float currentCold)
        {
            slider.value = currentCold;
        }
    }
}
