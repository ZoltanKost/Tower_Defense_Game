using UnityEngine;
using UnityEngine.UI;

public class TutorialHighlighting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteMask dynamicMask;
    [SerializeField] private SpriteMask staticMask;
    public void SetHighlightRectangle(Transform highlightBounds, bool _static)
    {
        SpriteMask mask;
        if (_static)
        {
            dynamicMask.gameObject.SetActive(false);
            staticMask.gameObject.SetActive(true);
            mask = staticMask;
        }
        else
        {
            dynamicMask.gameObject.SetActive(true);
            staticMask.gameObject.SetActive(false);
            mask = dynamicMask;
        }
        sprite.gameObject.SetActive(true);
        mask.transform.position = highlightBounds.position;
        mask.transform.localScale = highlightBounds.localScale;
    }
    public void Dehighlight()
    {
        sprite.gameObject.SetActive(false);
    }
}
