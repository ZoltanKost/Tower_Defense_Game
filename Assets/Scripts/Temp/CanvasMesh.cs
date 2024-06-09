using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CanvasRenderer))]
public class CanvasMesh : MonoBehaviour
{
    CanvasRenderer canvasRenderer;
    private void Awake() {
        canvasRenderer = GetComponent<CanvasRenderer>();
        Mesh mesh = canvasRenderer.GetMesh();
    }
}
