using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    float step = 360 / 8;
    public void SetDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, Vector2.right);
        if (angle < 0) angle += 360;
        spriteRenderer.sprite = sprites[(int)(angle/step)];
        Debug.Log(angle + " " + step);
    }
}
