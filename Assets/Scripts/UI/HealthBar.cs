using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Slider slider; 
    public virtual void Set(float value){
        slider.value = value;
    }
    public void Reset(){
        slider.value = 1;
    }
}