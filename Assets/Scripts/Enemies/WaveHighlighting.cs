using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveHighlighting : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private LineRenderer prefab;

    LineRenderer[] waveLines;
    LineRenderer[] shipLines;

    int waveCount = 0;
    int shipCount = 0;

    [SerializeField] private WaveHighlightTemplate templatePrefab;
    [SerializeField] private Sprite templateDefaultSprite;

    [SerializeField] private Transform arrowPrefab;

    private void Awake()
    {
        waveLines = new LineRenderer[8];
        for (int i = 0; i < 8; i++)
        {
            waveLines[i] = Instantiate(prefab,transform);
            waveLines[i].gameObject.SetActive(false);
        }
        shipLines = new LineRenderer[8];
        for (int i = 0; i < 8; i++)
        {
            shipLines[i] = Instantiate(prefab, transform);
            shipLines[i].gameObject.SetActive(false);
        }
    }
    public void SetHighlighting(List<Wave> waves, List<Ship> ships)
    {
        SetWaves(waves);
        SetShips(ships);
    }
    // fix
    public void SetWaves(List<Wave> waves)
    {
        int count = waves.Count;
        waveCount = count;
        if (waveLines.Length < count) ResizeWaves();
        for (int i = 0; i < count; i++)
        {
            var line = waveLines[i];
            line.gameObject.SetActive(true);
            var path = waves[i].Path;
            int c = path.Count;
            var points = new Vector3[path.Count];
            for (int l = 0; l < c; l++)
            {
                points[l] = path[l].pos;
            }
            line.positionCount = c;
            line.SetPositions(points);
        }
        for (int i = count; i < waveLines.Length; i++)
        {
            waveLines[i].gameObject.SetActive(false);
        }
    }
    // fix
    public void AddWaveLine(Wave wave)
    {
        waveCount++;
        if (waveCount >= waveLines.Length) ResizeWaves();
        var line = waveLines[waveCount];
        line.gameObject.SetActive(true);
        var path = wave.Path;
        int c = path.Count;
        var points = new Vector3[path.Count];
        for (int l = 0; l < c; l++)
        {
            points[l] = path[l].pos;
        }
        line.positionCount = c;
        line.SetPositions(points);
    }
    // fix
    public void SetShips(List<Ship> ships)
    {
        int count = ships.Count;
        shipCount = count;
        if (shipLines.Length < count) ResizeShips();
        for (int i = 0; i < count; i++)
        {
            var line = shipLines[i];
            line.gameObject.SetActive(true);
            var path = ships[i].path;
            int c = path.Count;
            var points = new Vector3[path.Count];
            for (int l = 0; l < c; l++)
            {
                points[l] = path[l].pos;
            }
            line.positionCount = c;
            line.SetPositions(points);
            AddWaveLine(ships[i].wave); 
        }
        for (int i = count; i < waveCount; i++)
        {
            shipLines[i].gameObject.SetActive(false);
        }
    }
    // fix
    public void ResizeWaves()
    {
        int oldL = waveLines.Length;
        int newL = waveLines.Length * 2;
        Array.Resize(ref waveLines, newL);
        for (int i = oldL; i < newL; i++)
        {
            waveLines[i] = Instantiate(prefab,transform);
            waveLines[i].gameObject.SetActive(false);
        }
    }
    // fix
    public void ResizeShips()
    {
        int oldL = shipLines.Length;
        int newL = shipLines.Length * 2;
        Array.Resize(ref shipLines, newL);
        for (int i = oldL; i < newL; i++)
        {
            shipLines[i] = Instantiate(prefab, transform);
            shipLines[i].gameObject.SetActive(false);
        }
    }
    float InExpo(float x)
    {
        return x == 0 ? 0f : (float)Math.Pow(4f, 10f * x - 8f);
    }
/*    public void RemoveLine(int index)
    {
        var temp = arrows[index];
        arrows[index] = arrows[--arrowCount];
        arrows[arrowCount] = temp;
        temp.gameObject.SetActive(false);
        arrowsIndexes[index] = arrowsIndexes[arrowCount];
        arrowsPaths[index] = arrowsPaths[arrowCount];
    }*/
}
