using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextWaveButton : MonoBehaviour{
    [SerializeField] private Button targetButton;
    public void Init(UnityAction start){
        if(targetButton == null) targetButton = GetComponent<Button>();
        Button.ButtonClickedEvent u_event = new();
        u_event.AddListener(start);
        targetButton.onClick = u_event;
    }
    
}