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

    [Serializable]
    public struct Pages
    {
        public int pageNumber;
        public string pageTitle;
        [TextArea(5, 10)] public string pageInstructions;
        public Sprite pageImage;
        public bool isComplete;
    }

    [SerializeField] Pages[] instructionPages;

    // Start is called before the first frame update
    void Start()
    {
        GameObject pageTemplate = transform.GetChild(0).gameObject;
        GameObject popPages;

        for (int i = 0; i < instructionPages.Length; i++)
        {
            popPages = Instantiate(pageTemplate, transform);
            popPages.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageNumber.ToString();
            popPages.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageTitle;
            popPages.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = instructionPages[i].pageInstructions;
            popPages.transform.GetChild(3).GetComponent<Image>().sprite = instructionPages[i].pageImage;

            popPages.transform.SetParent(MenuManager.transform.parent, false);
        }
    }
}
