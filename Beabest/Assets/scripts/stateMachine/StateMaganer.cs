﻿using UnityEngine;

public class StateMaganer : MonoBehaviour
{
    private IState currentlyRunningState;
    private IState previousState;

    public void ChangeState(IState newState)
    {
        if(currentlyRunningState != null)
            currentlyRunningState.Exit();

        previousState = currentlyRunningState;
        currentlyRunningState = newState;
        currentlyRunningState.Enter();
    }

    public void ExecuteStateUpdate()
    {
        var runningState = currentlyRunningState;
        if (runningState != null)
            runningState.Execute();
    }

    public void SwitchToPreviousState()
    {
        currentlyRunningState.Exit();
        currentlyRunningState = previousState;
        currentlyRunningState.Enter();
    }
}