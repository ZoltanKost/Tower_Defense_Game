using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHighlightTemplate : MonoBehaviour
{
    [SerializeField] private Vector2 borderOffset;
    public Transform pointArrow;
    public Image image;
    public TMP_Text count;
    public TMP_Text time;
    Camera mainCamera;
    Transform target;
    Vector3 destination;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public void SetWaveData(int _count, Transform _target, Vector3 _destination)
    {
        count.text = _count.ToString();
        gameObject.SetActive(true);
        target = _target;
        destination = _destination;
        time.gameObject.SetActive(true);
    }
    public void SetWaveData(int _count, Vector3 position)
    {
        count.text = _count.ToString();
        gameObject.SetActive(true);
        time.gameObject.SetActive(false);
    }
    public void Update()
    {
        Vector3 position = target.position;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
        
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, borderOffset.x, 0.9f);
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, borderOffset.y, 0.9f);

        if(viewportPoint.x > borderOffset.x && viewportPoint.x < 0.9f &&
            viewportPoint.y > borderOffset.y && viewportPoint.y < 0.9f)
        {
            pointArrow.gameObject.SetActive(false);
        }
        else
        {
            pointArrow.gameObject.SetActive(true);
        }

        viewportPoint = mainCamera.ViewportToWorldPoint(viewportPoint);
        viewportPoint.z = 0;
        transform.position = viewportPoint;
        time.text = Mathf.Floor((target.position - destination).magnitude).ToString();
    }
}