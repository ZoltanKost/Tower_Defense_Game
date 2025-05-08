using UnityEngine;
using System.IO;
[ExecuteInEditMode]
public class RotationScript : MonoBehaviour
{
    [SerializeField] Texture2D texture;
    RenderTexture renderTexture;
    [SerializeField] bool update;
    [SerializeField] bool save;
    [SerializeField] int x = 0, y = 0, width = 512, height = 512;
    public void Update()
    {
        if(texture == null)
        {
            texture = new Texture2D(width * 4, height * 2, TextureFormat.ARGB32, false);
        }
        if (save)
        {
            var png = ImageConversion.EncodeToPNG(texture);
            File.WriteAllBytes(Application.dataPath + "/Ship.png", png);
            Camera.main.targetTexture = null;
            Destroy(texture);
        }
        if (!update) return;
        update = false;

        Rect rect = new Rect(0, 0, width, height);

        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        Camera cam = Camera.main;
        cam.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cam.Render();
        
        texture.ReadPixels(rect, x, y);
        x += width;
        if (x >= width * 4)
        {
            x = 0;
            y += height;
        }
        texture.Apply();
    }
}
