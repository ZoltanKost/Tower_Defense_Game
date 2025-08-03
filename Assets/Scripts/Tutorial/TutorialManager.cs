using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private WorldManager worldManager;
    [SerializeField] private PlayerActionManager playerActionManager;
    [SerializeField] private TutorialStep[] steps;
    int currentStepIndex;

    public void StartTutorial()
    {
        currentStepIndex = 0;
        var step = steps[currentStepIndex];
        step.Prepare(Step);
    }

    public void Step()
    {
        Debug.Log(currentStepIndex);
        TutorialStep step = steps[currentStepIndex++];
        step.Execute();
        if(currentStepIndex < steps.Length)
        {
            step = steps[currentStepIndex];
            step.Prepare(Step);
        }
        else
        {
            EndTutorial();
        }
    }
    public void EndTutorial()
    {
        currentStepIndex = 0;
        gameObject.SetActive(false);
    }
}
