﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class UIFloor: MonoBehaviour
{
    [SerializeField] private Tilemap[] visuals;
    [SerializeField] private UITilemap[] renderers;
    public int floor;
    private const int SHADOWLAYER = 0;
    private const int GROUNDLAYER = 1;
    private const int GRASSLAYER = 2;
    private const int SANDLAYER = 3;
    private const int BRIDGELAYER = 4;
    private TileID ground
    {
        get
        {
            return floor == 0 ? TileID.Sand : TileID.Ground;
        }
    }
    private TileID grass
    {
        get
        {
            return floor == 0 ? TileID.None : TileID.Grass;
        }
    }
    public void Init(int cellsize, int width, int height)
    {
        renderers[SHADOWLAYER].Init(width, height, cellsize, StaticTiles.GetTile(TileID.Shadow));
        renderers[GROUNDLAYER].Init(width, height + 1, cellsize, StaticTiles.GetTile(TileID.Ground), Vector3Int.down);
        renderers[GRASSLAYER].Init(width, height, cellsize, StaticTiles.GetTile(TileID.Grass));
        renderers[SANDLAYER].Init(width, height, cellsize, StaticTiles.GetTile(TileID.Sand));
        renderers[BRIDGELAYER].Init(width, height, cellsize, StaticTiles.GetTile(TileID.Bridge));
    }
    public void ClearAllTiles()
    {
        for (int i = 0; i < visuals.Length; i++)
        {
            renderers[i].ClearAllTiles();
            visuals[i].ClearAllTiles();
        }
    }

    public void CreateGround(Vector3Int pos)
    {
        pos.z = 0;
        int gl = GROUNDLAYER;
        if (floor == 0) gl = SANDLAYER;
        SetTile(pos, SHADOWLAYER, TileID.Shadow);
        SetTile(pos, gl, ground);
        if (floor > 0)
        {
            SetTile(pos + Vector3Int.down, GROUNDLAYER, TileID.Rock);
            SetTile(pos, GRASSLAYER, grass);
            SetTile(pos + Vector3Int.down, SHADOWLAYER, TileID.Shadow);
        }
    }

    void SetTile(Vector3Int pos, int LAYER, TileID ID)
    {
        visuals[LAYER].SetTile(pos, StaticTiles.GetTile(ID));
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        int targetFloor = 4;
        switch (StaticTiles.GetID(tile))
        {
            case TileID.Sand:
                targetFloor = SANDLAYER;
                break;
            case TileID.Bridge:
                targetFloor = BRIDGELAYER;
                break;
            case TileID.BridgeOnGround:
                targetFloor = BRIDGELAYER;
                break;
        }
        SetTile(position, targetFloor, tile);
    }

    void SetTile(Vector3Int pos, int LAYER, TileBase tile)
    {
        visuals[LAYER].SetTile(pos, tile);
    }
    public void Activate()
    {
        //gameObject.SetActive(true);
        foreach (var r in renderers)
        {
            r.SetActive(true);
        }
    }
    public void Deactivate()
    {
        //gameObject.SetActive(false);
        foreach (var r in renderers)
        {
            r.SetActive(false);
        }
    }
}