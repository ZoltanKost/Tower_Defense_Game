using UnityEngine;
using UnityEngine.Tilemaps;

public class BridgeNeighbour : RuleTile.TilingRule.Neighbor{
    public const int RoadCollision = 3;
    public const int BridgeCollision = 4;
}
[CreateAssetMenu]
public class BridgeOnGround : RuleTile<BridgeNeighbour> {
    [SerializeField] private TileBase RoadTile;
    [SerializeField] private TileBase BridgeTile;
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case BridgeNeighbour.RoadCollision: return tile == RoadTile;
            case BridgeNeighbour.BridgeCollision: return tile == BridgeTile;
        }
        return base.RuleMatch(neighbor, tile);
    }
}