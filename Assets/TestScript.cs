using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScript : MonoBehaviour
{
    [SerializeField] private GroundUI groundUI;
    [SerializeField] private GroundArrayGenerator generator;
    [SerializeField] private Floor floor;
    public TileBase FOAM;
    public TileBase GROUND;
    public TileBase ROCK;
    public TileBase GRASS;
    public TileBase SHADOW;
    public TileBase LADDER;
    public TileBase GRASS_SHADOW;
    public TileBase SAND;
    public TileBase BRIDGE;
    public TileBase BRIDGE_ON_GROUND;
    void Start()
    {
        StaticTiles.Init();
        StaticTiles.Bind(SHADOW, TileID.Shadow);
        StaticTiles.Bind(GROUND, TileID.Ground);
        StaticTiles.Bind(GRASS, TileID.Grass);
        StaticTiles.Bind(FOAM, TileID.Foam);
        StaticTiles.Bind(ROCK, TileID.Rock);
        StaticTiles.Bind(GRASS_SHADOW, TileID.GrassPieces);
        StaticTiles.Bind(SAND, TileID.Sand);
        StaticTiles.Bind(LADDER, TileID.Ladder);
        StaticTiles.Bind(BRIDGE, TileID.Bridge);
        StaticTiles.Bind(BRIDGE_ON_GROUND, TileID.BridgeOnGround);
        groundUI.Init(0, Regenerate,3,3);
        floor.Init(0,$"GroundUI");
    }

    void Regenerate(int id)
    {
        var ga = generator.GenerateGA();
        groundUI.SetGroundArray(ga);
        groundUI.ActivateVisuals();
        floor.ClearAllTiles();
        foreach (GACell v in ga.grounds)
        {
            floor.CreateGround(v.position);
        }
    }
}
