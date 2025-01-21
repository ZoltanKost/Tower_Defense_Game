using UnityEngine;

public class SpellObject : MonoBehaviour
{
    public CustomAnimator animator;
    public SpellData data;
    public SpellSpawnData spawnData;
    public Vector3 position;
    public int id;
    public bool damage;
    public bool spawn;
    public bool remove;
    public void Init(Animation[] array, int index, Vector3 _position, SpellData spellData)
    {
        position = _position;
        transform.SetPositionAndRotation(position, Quaternion.identity);
        gameObject.SetActive(true);
        id = index;
        animator.InitFromAnimArray(array);
        data = spellData;
        damage = false;
        remove = false;
        spawn = false;
    }
    public void Init(Animation[] array, int index, Vector3 _position, SpellData spellData, SpellSpawnData spellSpawnData)
    {
        position = _position;
        transform.SetPositionAndRotation(position, Quaternion.identity);
        gameObject.SetActive(true);
        id = index;
        animator.InitFromAnimArray(array);
        data = spellData;
        spawnData = spellSpawnData;
        damage = false;
        remove = false;
        spawn = false;
    }
    public void Init(CustomAnimator animator, int index, Vector3 _position, SpellData spellData)
    {
        position = _position;
        transform.SetPositionAndRotation(position, Quaternion.identity);
        gameObject.SetActive(true);
        id = index;
        animator.InitFromPrefab(animator);
        data = spellData;
        damage = false;
        remove = false;
        spawn = false;
    }
    public void Init(CustomAnimator animator, int index, Vector3 _position, SpellData spellData, SpellSpawnData spellSpawnData)
    {
        position = _position;
        transform.SetPositionAndRotation(position, Quaternion.identity);
        gameObject.SetActive(true);
        id = index;
        animator.InitFromPrefab(animator);
        data = spellData;
        spawnData = spellSpawnData;
        damage = false;
        remove = false;
        spawn = false;
    }
    public void DamageCallback()
    {
        Debug.Log("Damage CAllback");
        damage = true;
    }
    public void SpawnCallback()
    {
        Debug.Log("Spawn CAllback");
        spawn = true;
    }
    public void RemoveCallback()
    {
        Debug.Log("Remove CAllback");
        remove = true;
    }
}
