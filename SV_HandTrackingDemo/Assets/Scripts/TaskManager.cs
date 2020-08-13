﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public Button nextBtn;
    int currentfinishedObjs;
    int taskCompleteObjs;

    public pageSystem pageSys;
    [HideInInspector] public Timer timer;
    private OVRGrabbable isTaskComplete;

    private void Start()
    {
        Reset();
    }

    public void PieceFinished()
    {
        currentfinishedObjs++;
        Debug.Log("<b><color=white>Currently Finished Task Segments: </color></b>" + currentfinishedObjs + 
            ". There are " + (taskCompleteObjs - currentfinishedObjs) + " segments remaining.");

        if (currentfinishedObjs == taskCompleteObjs)
        {
            nextBtn.interactable = true;
            nextBtn.GetComponent<BoxCollider>().enabled = true;
            timer.timerEnd();
            Debug.Log("<b><color=white>Task Complete.</color></b>");
        }
    }

    public void Reset()
    {
        currentfinishedObjs = 0;
        taskCompleteObjs = pageSys.returnCurrentPage().moveableTaskObjects.Length;
        nextBtn.interactable = false;
        nextBtn.GetComponent<BoxCollider>().enabled = true;
        isTaskComplete.isTaskComplete = false;
    }
}
