using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PressButtonInGridLayoutStep : TutorialStep
{
    [SerializeField] private GridLayoutGroup gridLayout;
    UnityAction action;
    public override void Execute()
    {
        base.Execute();
        foreach (var x in gridLayout.GetComponentsInChildren<Button>())
        {
            x.onClick.RemoveListener(action);
        }
        highlighting.Dehighlight();
    }
    public override void Prepare(Action step)
    {
        base.Prepare(step);
        foreach (var x in gridLayout.GetComponentsInChildren<Button>())
        {
            action = new UnityAction(step);
            x.onClick.AddListener(action);
        }
        highlighting.SetHighlightRectangle(highlightArea.transform, false);
        if (showText)
        {
            dialogue.gameObject.SetActive(true);
            dialogue.SetText(Text);
        }
        else
        {
            dialogue.gameObject.SetActive(false);
        }
    }
}
