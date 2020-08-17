using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    //Variables
    public Button nextBtn;
    int currentfinishedObjs;
    int taskCompleteObjs;

    public pageSystem pageSys;
    public Timer timer;

    private void Start()
    {
        //Reset allows it to have its initial values.
        Reset();
    }

    public void PieceFinished()
    {
        //Add the Object to the currentfinishedObjs.
        currentfinishedObjs++;
        Debug.Log("<b><color=white>Currently Finished Task Segments: </color></b>" + currentfinishedObjs + 
            ". There are " + (taskCompleteObjs - currentfinishedObjs) + " segments remaining.");

        //If all the objects relating to the task have completed their elements.
        if (currentfinishedObjs == taskCompleteObjs)
        {
            //Allow the user to move onto the next task.
            nextBtn.interactable = true;
            nextBtn.GetComponent<BoxCollider>().enabled = true;
            //End the timer.
            timer.timerEnd();
            Debug.Log("<b><color=white>Task Complete.</color></b>");
        }
    }

    //Reset tasks.
    public void Reset()
    {
        //Reset tracked objects.
        currentfinishedObjs = 0;
        taskCompleteObjs = pageSys.returnCurrentPage().moveableTaskObjects.Length;
        //Start the timer.
        timer.timerStart();
        //Remove the functionality of the next button until the task is complete.
        nextBtn.interactable = false;
        nextBtn.GetComponent<BoxCollider>().enabled = false;
    }
}
