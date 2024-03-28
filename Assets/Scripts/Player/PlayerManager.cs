using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public delegate void OnPlayerLost();
    OnPlayerLost onPlayerLost;
    [SerializeField] int HP = 100;
    public void Init(OnPlayerLost onPlayerLost){
        this.onPlayerLost = onPlayerLost;
    }
    public void Damage(int damage){
        HP -= damage;
        if(HP <= 0){
            onPlayerLost?.Invoke();
        }
    }
}