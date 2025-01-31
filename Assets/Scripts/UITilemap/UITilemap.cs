using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(Tilemap))]
public class UITilemap : MonoBehaviour {
    static Mesh NullMesh;
    [SerializeField] private Material material;
    [SerializeField] private Texture texture;
    [SerializeField] private Mesh mesh;
    [SerializeField] private int width,height;
    private int cellSize;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;
    private float cs;
    private Tilemap tilemap;
    CanvasRenderer canvasRenderer;
    Vector3Int offset;
    public void Init(int width, int height, int cellsize, TileBase tile, Vector3Int offset = default)
    {
        if(NullMesh == null) NullMesh = new Mesh();
        this.offset = offset;
        transform.localPosition = offset * cellsize;
        tilemap = GetComponent<Tilemap>();
        Tilemap.tilemapTileChanged += UpdateTilemap;
        this.width = width;
        this.height = height;
        cellSize = cellsize;
        canvasRenderer = GetComponent<CanvasRenderer>();
        vertices = new Vector3[width * height * 4];
        uv = new Vector2[width * height * 4];
        triangles = new int[width * height * 6];
        cs = cellSize;
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                int i = x + y * width;
                Vector3 v = new Vector3(x * cs, y * cs, 0);
                vertices[i * 4] = v;
                vertices[i * 4 + 1] = v + new Vector3(0,cs);
                vertices[i * 4 + 2] = v + new Vector3(cs,cs);
                vertices[i * 4 + 3] = v + new Vector3(cs,0);

                triangles[i * 6] = i * 4;
                triangles[i * 6 + 1] = i * 4 + 1;
                triangles[i * 6 + 2] = i * 4 + 2;

                triangles[i * 6 + 3] = i * 4;
                triangles[i * 6 + 4] = i * 4 + 2;
                triangles[i * 6 + 5] = i * 4 + 3;
            }
        }
        mesh = new Mesh
        {
            name = "Mesh",
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };
        TileData tileData = default;
        tile.GetTileData(Vector3Int.zero,tilemap,ref tileData);
        texture = tileData.sprite.texture;
        canvasRenderer.SetMesh(mesh);
        canvasRenderer.SetMaterial(material, texture);
    }

    private void UpdateTilemap(Tilemap tilemap, Tilemap.SyncTile[] arg2)
    {
        if (tilemap != this.tilemap) return;
        foreach (Tilemap.SyncTile tile in arg2) 
        {
            if(tile.position.x - offset.x < 0 || tile.position.x - offset.x >= width || tile.position.y - offset.y < 0  || tile.position.y - offset.y >= height) continue;
            int index = ((tile.position.x - offset.x) + (tile.position.y - offset.y) * width) * 4;

            Vector2 uv0 = Vector2.zero;
            Vector2 uv1 = Vector2.zero;
            Vector2 uv2 = Vector2.zero;
            Vector2 uv3 = Vector2.zero;
            
            if (tile.tileData.sprite != null)
            {
                Sprite sprite = tile.tileData.sprite;
                uv0 = sprite.uv[2];
                uv1 = sprite.uv[0];
                uv2 = sprite.uv[1];
                uv3 = sprite.uv[3];
            }

            uv[index] = uv0;
            uv[index + 1] = uv1;
            uv[index + 2] = uv2;
            uv[index + 3] = uv3;
            
        }
        mesh.uv = uv;
        canvasRenderer.SetMesh(mesh);
    }

    public void ClearAllTiles()
    {
        uv = new Vector2[width * height * 4];
        mesh.uv = uv;
        canvasRenderer.SetMesh(mesh);
    }

    public void SetActive(bool active)
    {
        if(active == true)
        {
            canvasRenderer.SetMesh(mesh);
        }
        else
        {
            canvasRenderer.SetMesh(NullMesh);
        }
    }
}