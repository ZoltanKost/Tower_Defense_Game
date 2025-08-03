using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PressButtonTutorialStep : TutorialStep
{
    [SerializeField] private Button target;
    [SerializeField] private protected UnityAction action;
    public override void Execute()
    {
        Debug.Log("PressButton step!");
        base.Execute();
        target.onClick.RemoveListener(action);
        highlighting.Dehighlight();
    }

    public override void Prepare(Action step)
    {
        Debug.Log("Preparing PressButton step...");
        base.Prepare(step);
        action = new UnityAction(step);
        target.onClick.AddListener(action);
        highlighting.SetHighlightRectangle(highlightArea.transform, false);
    }
}
