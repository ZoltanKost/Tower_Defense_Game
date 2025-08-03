using System;
using UnityEngine;

public class WaitLevelStep : TutorialStep
{
    [SerializeField] private EnemyManager enemyManager;
    Action step;
    public override void Prepare(Action _step)
    {
        Debug.Log("Preparing waitForLev step...");
        step = _step;
        enemyManager.onWaveFinishedTutorialCallback += step;
    }

    public override void Execute()
    {
        Debug.Log("WaitForLev step!");
        enemyManager.onWaveFinishedTutorialCallback -= step;
    }
}
