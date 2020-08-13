using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class pageSystem : MonoBehaviour
{
    //references
    public GameObject MenuManager;
    public GameObject pageTemplate;

    //assign the template parameters in the inspector for easy editing of values
    [SerializeField] private TextMeshProUGUI tempPageNumber;
    [SerializeField] private TextMeshProUGUI tempPageTitle;
    [SerializeField] private TextMeshProUGUI tempPageDescription;
    [SerializeField] private Image tempPageImage;

    //struct to allow multiple customiseable serialized objects in the inspector
    [Serializable]
    public struct Page
    {
        public int pageNumber;
        public string pageTitle;
        [TextArea(5, 10)] public string pageInstructions; //textarea allows for bigger area to type
        public Sprite pageImage;
        public GameObject[] moveableTaskObjects;
        public bool taskComplete;

        [HideInInspector]
        public GameObject pageTemplate;
    }

    //track the current page
    private int pageIndex;

    //navigation buttons
    public Button prevBtn, nextBtn, homeBtn, finishBtn;

    //pages list
    [SerializeField] Page[] instructionPages;

    private TaskManager tasks;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("<b><color=white>Number of Task Instructions Found: </color></b>" + instructionPages.Length);

        //old way of doing things; keeping as reference (12/08 11:20)
        #region "oldInstantiateCodeRef"

        //GameObject popPages;

        //old way of doing things; keeping as reference (12/08 11:20)
        ////for the number of tasks needed
        //for (int i = 0; i < instructionPages.Length; i++)
        //{
        //    //instantiate the template page and populate the children with the correct information supplied
        //    //from the inspector
        //    popPages = Instantiate(pageTemplate, transform);
        //    popPages.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageNumber.ToString();
        //    popPages.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageTitle;
        //    popPages.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageInstructions;
        //    popPages.transform.GetChild(3).GetComponent<Image>().sprite = instructionPages[i].pageImage;

        //    popPages.transform.SetParent(MenuManager.transform.parent, false);
        //    popPages.SetActive(false);

        //    pageIndex = i;

        //    //Debug.Log(i);
        //}
        ////need to figure out how to hook up the UI buttons to work with each of these pages
        ////perhaps could instantiate them also and do i+1 and i-1 for the page numbers with a check for 0 and .length for min and max
        ///
        #endregion

        prevBtn.interactable = false;
        finishBtn.interactable = false;
        finishBtn.GetComponent<BoxCollider>().enabled = false;
    }

    public Page returnCurrentPage()
    {
        return instructionPages[pageIndex];
    }

    private void OnEnable()
    {
        //set the page to display the first page values (home page)
        tempPageNumber.text = instructionPages[0].pageNumber.ToString() + " of " + instructionPages.Length.ToString();
        tempPageTitle.text = instructionPages[0].pageTitle;
        tempPageDescription.text = instructionPages[0].pageInstructions;
        tempPageImage.sprite = instructionPages[0].pageImage;
    }

    //using SetPage we can set the parameters to be correct for what page we need i.e. 'index'
    private void SetPage(int index)
    {
        tempPageNumber.text = instructionPages[index].pageNumber.ToString() + " of " + instructionPages.Length.ToString();
        tempPageTitle.text = instructionPages[index].pageTitle;
        tempPageDescription.text = instructionPages[index].pageInstructions;
        tempPageImage.sprite = instructionPages[index].pageImage;

        pageIndex = index;
    }

    public void prevPage()
    {
        Debug.Log("Clicked 'Previous Page'.");

        //if the array is 0, it's the home page, so cancel
        if (pageIndex == 0) return;

        //lower the index by 1 
        pageIndex--;

        //if its 0 after lowering the index, make the button uninteractable
        if (pageIndex == 0)
        {
            prevBtn.interactable = false;
        }

        else
        {
            prevBtn.interactable = true;
        }

        //set the page
        SetPage(pageIndex);
    }

    public void nextPage()
    {
        Debug.Log("Clicked 'Next Page'.");

        //if the array is trying to look for a page higher than the pages set, it wont find it due to not existing, so cancel
        if (pageIndex == instructionPages.Length - 1) return;

        //if next is pressed, prev must become available and interactable
        prevBtn.interactable = true;

        //raise the index by 1
        pageIndex++;

        //check after index raised
        if (pageIndex == instructionPages.Length - 1)
        {
            //if its the last array, make the next button uninteractable
            nextBtn.interactable = false;
            nextBtn.GetComponent<BoxCollider>().enabled = false;
            finishBtn.interactable = true;
            finishBtn.GetComponent<BoxCollider>().enabled = true;
        }

        //set the page
        SetPage(pageIndex);
    }

    public void homePage()
    {
        Debug.Log("Clicked 'Home Page'.");

        //set the page to the home page value
        SetPage(0);

        //reset index
        pageIndex = 0;
        prevBtn.interactable = false;

        
    }
}
