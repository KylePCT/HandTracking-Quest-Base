using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class pageSystem : MonoBehaviour
{
    public GameObject MenuManager;
    public GameObject pageTemplate;

    //struct to allow multiple customiseable serialized objects in the inspector
    [Serializable]
    public struct Page
    {
        public int pageNumber;
        public string pageTitle;
        [TextArea(5, 10)] public string pageInstructions; //textarea allows for bigger area to type
        public Sprite pageImage;
        public bool isComplete;

        public GameObject pageTemplate;
    }

    private int pageIndex;

    //navigation buttons
    public Button prevBtn, nextBtn, homeBtn;

    //pages list
    [SerializeField] Page[] instructionPages;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("<b><color=white>Number of Task Instructions Found: </color></b>" + instructionPages.Length);

        GameObject popPages;

        //for the number of tasks needed
        for (int i = 0; i < instructionPages.Length; i++)
        {
            //instantiate the template page and populate the children with the correct information supplied
            //from the inspector
            popPages = Instantiate(pageTemplate, transform);
            popPages.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageNumber.ToString();
            popPages.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageTitle;
            popPages.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageInstructions;
            popPages.transform.GetChild(3).GetComponent<Image>().sprite = instructionPages[i].pageImage;

            prevBtn.onClick.AddListener(prevPage);
            nextBtn.onClick.AddListener(nextPage);
            homeBtn.onClick.AddListener(homePage);

            popPages.transform.SetParent(MenuManager.transform.parent, false);
            popPages.SetActive(false);
        }
        //need to figure out how to hook up the UI buttons to work with each of these pages
        //perhaps could instantiate them also and do i+1 and i-1 for the page numbers with a check for 0 and .length for min and max
    }

    public void prevPage()
    {
        Debug.Log("Clicked 'Previous Page'.");

        instructionPages[pageIndex].pageTemplate.SetActive(false);
        pageIndex--;

        if (pageIndex <= 0)
        {
            homePage();
        }
    }

    public void nextPage()
    {
        Debug.Log("Clicked 'Next Page'.");

        instructionPages[pageIndex].pageTemplate.SetActive(false);
        pageIndex++;

        if (pageIndex > instructionPages.Length - 1)
        {
            homePage();
        }
    }

    public void homePage()
    {
        Debug.Log("Clicked 'Home Page'.");
    }
}
