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

    // Start is called before the first frame update.
    void Start()
    {
        //Get the Button element from the connected object.
        attachedButton = GetComponent<Button>();
        currentlyPoked = false;
    }

    // Update is called once per frame.
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the FingerTip collision from the finger of the hands enters the collision of the button...
        if (other.tag == "FingerTip" && currentlyPoked == false)
        {
            //Change the colour and set 'poked' to true; invoke the 'poked' event.
            attachedButton.image.color = Color.red;
            whenPoked.Invoke();
            currentlyPoked = true;
        }
    }

    //If the FingerTip collision from the finger of the hands leaves the collision of the button...
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FingerTip" && currentlyPoked == true)
        {
            //Reset the colour and set 'poked' to false.
            attachedButton.image.color = Color.white;
            currentlyPoked = false;
        }
    }
}
