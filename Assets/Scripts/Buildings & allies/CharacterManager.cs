using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private Character prefab;
    [SerializeField] private FloorManager floorManager;
    public Character[] characters;
    int Count;
    public void Init()
    {
        Count = 0;
        characters = new Character[16];
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = Instantiate(prefab, transform);
            characters[i].gameObject.SetActive(false);
        }
    }

    public void Spawn(Vector3 position, CharacterData characterData)
    {
        characters[Count++].Init(position, Vector2Int.zero, characterData);
        if(Count >= characters.Length)
        {
            Resize();
        }
    }
    public void Resize()
    {
        Array.Resize(ref characters, Count * 2);
        for(int i = Count; i < characters.Length; i++)
        {
            characters[i] = Instantiate(prefab, transform);
            characters[i].gameObject.SetActive(false);
        }
    }
}
public struct CharacterData
{
    public AttackType AttackType;
    public int damage;
    public ProjectileData ProjectileData;
    public int HP;
    public Animation[] animations;
}