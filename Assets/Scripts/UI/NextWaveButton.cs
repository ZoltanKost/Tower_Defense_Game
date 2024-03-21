using UnityEngine;
using UnityEngine.UI;

public class NextWaveButton : MonoBehaviour{
    [SerializeField] private Button targetButton;
    [SerializeField] private Pathfinding path;
    [SerializeField] private GroundPiecesUIManager groundPiecesUIManager;
    [SerializeField] private EnemyManager enemy;
    [SerializeField] private PlayerInputManager player;
    void Awake(){
        targetButton = GetComponent<Button>();
        Button.ButtonClickedEvent u_event = new();
        u_event.AddListener(Register);
        targetButton.onClick = u_event;
    }
    
    public void Register(){
        if(!path.FindPathToCastle()) return;
        transform.parent.gameObject.SetActive(false);
        groundPiecesUIManager.Hide();
        player.Deactivate();
        enemy.Activate();
    }
}