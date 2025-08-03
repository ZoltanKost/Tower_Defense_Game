using System;
using UnityEngine;

public class BuildTowerTutorialStep : TutorialStep
{
    [SerializeField] private PlayerActionManager actionManager;
    [SerializeField] private ActionMode targetMode;
    Action action;
    public override void Execute()
    {
        base.Execute();
        Debug.Log("BuildTow Executed!");
        actionManager.buildingFinishedCallback -= ExecutedBuildingAction;
    }

    public override void Prepare(Action step)
    {
        Debug.Log("Preparing BuildTow step...");
        base.Prepare(step);
        action = step;
        actionManager.buildingFinishedCallback += ExecutedBuildingAction;
        if (highlight) highlighting.SetHighlightRectangle(highlightArea.transform,true);
    }

    public void ExecutedBuildingAction(ActionMode mode)
    {
        Debug.Log(mode);
        if (targetMode == mode)
        {
            Debug.Log("step executed");
            action?.Invoke();
        }
    }

}
