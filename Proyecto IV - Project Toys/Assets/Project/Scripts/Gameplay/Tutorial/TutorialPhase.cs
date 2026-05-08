using System;
using UnityEngine;

public class TutorialPhase
{
    public enum Status
    {
        NotStarted,
        InProgress,
        Completed
    }
    public Status status;
    public bool isCompleted;
    
    private Func<bool> completionCondition;
    
    public TutorialPhase(Func<bool> completionCondition)
    {
        this.completionCondition = completionCondition;
        status = Status.NotStarted;
        isCompleted = false;
    }
    
    public void StartPhase()
    {
        if (status == Status.NotStarted)
        {
            status = Status.InProgress;
        }
    }
    
    public void UpdatePhase()
    {
        if (status == Status.InProgress && completionCondition())
        {
            CompletePhase();
        }
    }
    
    private void CompletePhase()
    {
        status = Status.Completed;
        isCompleted = true;
    }
}
