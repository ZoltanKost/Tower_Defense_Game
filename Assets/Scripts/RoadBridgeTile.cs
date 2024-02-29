using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadNeighbour : RuleTile.TilingRule.Neighbor{
    public const int Collision = 3;
}

[CreateAssetMenu]
public class RoadBridgeTile : RuleTile<RoadNeighbour> {
    [SerializeField] private TileBase compatible;
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case RoadNeighbour.Collision: return compatible == tile;
        }
        return base.RuleMatch(neighbor, tile);
    }
}