using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    [SerializeField] private CustomAnimator animator;
    public Vector2Int gridPosition;
    public Vector2Int gridDestination;
    Vector3 dir;
    public List<int> commands;
    int currentCommand;
    float Speed;
    public void Init(Vector3 position, Vector2Int gridPos, CharacterData data)
    {
        transform.position = position;
        gridPosition = gridPos;
    }

    public void ExecuteCommand(float delta)
    {
        switch (currentCommand)
        {
            case 0:
                return;
            case 1:
                dir = new Vector3(gridDestination.x - gridPosition.x, gridDestination.y - gridPosition.y).normalized;
                transform.position += Speed * delta * dir;
                break;
        }
    }

    public void PopCommands()
    {
        if (commands[0] > 0) return;
        currentCommand = commands[0];
        commands.RemoveAt(0);
    }
    public Sprite GetSprite()
    {
        return animator.spriteRenderer.sprite;
    }
}