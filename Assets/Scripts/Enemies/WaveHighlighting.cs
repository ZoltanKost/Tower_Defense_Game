using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveHighlighting : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Canvas arrowParent;

    [SerializeField] private WaveHighlightTemplate templatePrefab;
    [SerializeField] private Sprite templateDefaultSprite;

    [SerializeField] private Transform arrowPrefab;
    
    [SerializeField] private float arrowOffset = 0.2f;
    [SerializeField] private Vector3 offset = Vector3.down * 0.2f;
    [SerializeField] private float arrowSpeed = 1f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private int arrowNumber = 3;
    public bool needResize;
    [SerializeField] private Ease moveEase = Ease.Linear;
    [SerializeField] private Ease rotationEase = Ease.InSine;

    private List<WaveHighlightTemplate> templates = new();
    private Transform[] arrows;
    private int[] arrowsIndexes;
    private List<PathCell>[] arrowsPaths;

    Wave currentWave;
    int waveIndex;
    int pathIndex;

    int arrowCount;

    public void Awake()
    {
        arrows = new Transform[16];
        arrowsIndexes = new int[16];
        arrowsPaths = new List<PathCell>[16];
        for (int i = 0; i < 16; i++)
        {
            arrows[i] = Instantiate(arrowPrefab, arrowParent.transform);
            arrows[i].gameObject.SetActive(false);
        }
    }
    public void SetWaves(List<Wave> waves)
    {
        int tempCount = templates.Count;
        int waveCount = waves.Count;
        if (waveCount == 0) return;
        Debug.Log("Setting wave");
        int i = 0;
        for (;i < tempCount && i < waveCount; i++)
        {
            templates[i].SetWaveData(waves[i].count, templateDefaultSprite, waves[i].Path[waves[i].Path.Count - 1].pos);
        }
        while (i < waveCount)
        {
            var newTemplate = Instantiate(templatePrefab);
            newTemplate.SetWaveData(waves[i].count, templateDefaultSprite, waves[i].Path[waves[i].Path.Count - 1].pos);
            templates.Add(newTemplate);
            i++;
        }
        while (i < tempCount)
        {
            templates[i].gameObject.SetActive(false);
            i++;
        }
        waveIndex = 0;
        pathIndex = 0;
        currentWave = waves[waveIndex];
    }
    public void ClearWaves()
    {
        arrowCount = 0;
        foreach (var t in templates)
        {
            t.gameObject.SetActive(false);
        }
    }
    /*void Update()
    {
        if(arrowCount <= 0 && currentWave != null && currentWave.Path != null)
        {
            waveIndex++;
            if (waveIndex >= enemyManager.waves.Count)
            {
                waveIndex = 0;
            }
            currentWave = enemyManager.waves[waveIndex];
            SendArrows();
        }
    }*/
    private void FixedUpdate()
    {
        UpdateArrows(Time.fixedDeltaTime);
    }
    public void SendArrows()
    {
        //Debug.Log("Sending arrows: " + currentWave.Path.Count * arrowNumber);
        //string s = "";
        var path = currentWave.Path;
        {
            if (arrowCount + arrowNumber >= arrows.Length) 
            {
                //s += $"count: {arrowCount}; availible: {arrows.Length};";
                Resize(); 
            }
            Vector3 position = path[path.Count - 1].pos;
            for (int i = arrowCount; i < arrowCount + arrowNumber; i++)
            {
                arrowsIndexes[i] = path.Count - 1;
                arrowsPaths[i] = path; 
                arrows[i].gameObject.SetActive(true);
                float z = 90f;
                //s += $"{i}: {position} ";
                arrows[i].SetPositionAndRotation(position, Quaternion.Euler(0, 0, z));
                position += offset;
            }
            arrowCount += arrowNumber;
            //s+= "\n";
        }
        //Debug.Log(s);
    }
    public void UpdateArrows(float dt)
    {
        for (int i = 0; i < arrowCount; i++)
        {
            List<PathCell> paths = arrowsPaths[i];
            int index = arrowsIndexes[i];
            Vector3 moveDirection = paths[index].pos - arrows[i].position;
            Vector3 totalMove = default;
            if (index < paths.Count - 1) { totalMove = paths[index].pos - paths[index + 1].pos; }
            moveDirection.z = 0;
            Vector3 turnDirection = paths[index - 1].pos - paths[index].pos;
            turnDirection = Vector3.right - turnDirection;
            float z = -90f * turnDirection.y;
            if (turnDirection.x > 1) z += 180;
            Vector3 move = dt * arrowSpeed * moveDirection.normalized;
            float ease = totalMove.magnitude - moveDirection.magnitude;
            //Debug.Log($"{z} : {turnDirection} , move: {moveDirection.magnitude},ease: {ease} total:{totalMove} nextPoint: {paths[index].pos}; move:{move}");
            arrows[i].position = arrows[i].position + move;
            arrows[i].rotation = 
                Quaternion.Lerp(arrows[i].rotation, Quaternion.Euler(0,0,z), 
                index < paths.Count - 1?InExpo(ease) :1f);
            if ((moveDirection).magnitude < 0.1f)
            {
                arrowsIndexes[i]--;
            }
            if(arrowsIndexes[i] < 1)
            {
                RemoveArrow(i);
                i--;
            }
        }
    }
    float InExpo(float x)
    {
        return x == 0 ? 0f : (float)Math.Pow(4f, 10f * x - 8f);
    }
    public void RemoveArrow(int index)
    {
        var temp = arrows[index];
        arrows[index] = arrows[--arrowCount];
        arrows[arrowCount] = temp;
        temp.gameObject.SetActive(false);
        arrowsIndexes[index] = arrowsIndexes[arrowCount];
        arrowsPaths[index] = arrowsPaths[arrowCount];
    }
    public void Resize()
    {
        Array.Resize(ref arrows, arrows.Length * 2);
        Array.Resize(ref arrowsIndexes, arrows.Length);
        Array.Resize(ref arrowsPaths, arrows.Length);
        for (int i = arrowCount; i< arrows.Length; i++)
        {
            arrows[i] = Instantiate(arrowPrefab, arrowParent.transform);
            arrows[i].gameObject.SetActive(false);
        }
    }
}
