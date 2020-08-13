using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class pokeButtonInteraction : MonoBehaviour
{
    private Button attachedButton;
    private bool currentlyPoked;

    public UnityEvent whenPoked;

    // Start is called before the first frame update
    void Start()
    {
        attachedButton = GetComponent<Button>();
        currentlyPoked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FingerTip" && currentlyPoked == false)
        {
            attachedButton.image.color = Color.red;
            whenPoked.Invoke();
            currentlyPoked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FingerTip" && currentlyPoked == true)
        {
            attachedButton.image.color = Color.white;
            currentlyPoked = false;
        }
    }
}
