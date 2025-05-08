using UnityEngine;
using System.IO;

public class RenderTextureTest : MonoBehaviour
{

    [SerializeField] private Transform ship;
    public Vector3 shipLookRotation = new Vector3(0, 0.5f, -1f);
    public Vector3[] destinations;
    void Start()
    {
        int width = 512, height = 512;
        RenderTexture texture = new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);
        Camera cam = Camera.main;
        cam.targetTexture = texture;
        RenderTexture.active = texture;
        Rect rect = new Rect(0, 0, width, height);
        Texture2D textureCpu = new Texture2D(width * 4,height * 2,TextureFormat.ARGB32,false);
        int x = 0, y = 0;
        foreach (var destination in destinations)
        {
            ship.rotation = Quaternion.LookRotation((destination - ship.position), shipLookRotation);// ().normalized * dt * ships[i].speed;
            cam.Render();
            textureCpu.ReadPixels(rect,x,y);
            x += width;
            if(x >= width * 4)
            {
                x = 0;
                y += height;
            }
            textureCpu.Apply();
            //Sprite sprite = Sprite.Create(textureCpu,);
        }
        var png = ImageConversion.EncodeToPNG(textureCpu);
        File.WriteAllBytes(Application.dataPath + "/Ship.png", png);
        cam.targetTexture = null;
        Destroy(texture);

    }
}