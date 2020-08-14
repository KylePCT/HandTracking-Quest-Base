using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public Button nextBtn;
    int currentfinishedObjs;
    int taskCompleteObjs;

    public pageSystem pageSys;
    public Timer timer;

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

    //Reset tasks
    public void Reset()
    {
        currentfinishedObjs = 0;
        taskCompleteObjs = pageSys.returnCurrentPage().moveableTaskObjects.Length;
        timer.timerStart();
        nextBtn.interactable = false;
        nextBtn.GetComponent<BoxCollider>().enabled = false;
    }
}
