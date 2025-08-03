using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private TMP_Text textObject;
    string _string;
    int i;
    WaitForSeconds wait;
    public void SetText(string text)
    {
        i = 0;
        _string = text;
        textObject.text = "";
        wait = new WaitForSeconds(speed);
        StartCoroutine(StepText());
    }
    public IEnumerator StepText()
    {
        while (true)
        {
            yield return wait;
            if (i >= _string.Length) yield break;
            textObject.text += _string[i];
            i++;
        }
    }
}
