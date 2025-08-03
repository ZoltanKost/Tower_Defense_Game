using System;
using UnityEngine;

[Serializable]
public class TutorialStep : MonoBehaviour
{
    public DialogueWindow dialogue;
    public WorldManager worldManager;
    public bool pauseLevel;
    public bool highlight;
    public bool staticHighlightingPosition;
    public bool showText;
    public string Text;
    public TutorialHighlighting highlighting;
    public SpriteRenderer highlightArea;
    public virtual void Execute()
    {
        if (pauseLevel) worldManager.StartLevelTutorialStep();
        dialogue.gameObject.SetActive(false);
    }
    public virtual void Prepare(Action step)
    {
        dialogue.gameObject.SetActive(showText);
        if (showText)
        {
            dialogue.SetText(Text);
        }
        if (pauseLevel) worldManager.StopLevelTutorialStep();
    }
}