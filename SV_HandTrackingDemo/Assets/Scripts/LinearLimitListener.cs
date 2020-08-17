using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearLimitListener : MonoBehaviour
{
    //Get the configurable joint parameters.
    public GameObject ConfigJoint_Obj;
    private ConfigurableJoint configJoint;

    private Vector3 initialPos;
    private Vector3 endPos;
    private float linearLimit;

    //Event called on min reached.
    public UnityEvent OnMinLimitReached;
    //Event called on max reached.
    public UnityEvent OnMaxLimitReached;

    // Start is called before the first frame update
    void Start()
    {
        //Save its initial position.
        initialPos = transform.position;
        //Get the limit value.
        linearLimit = GetComponent<ConfigurableJoint>().linearLimit.limit;

        //Save the end position and save the 'y' value with the limit value added.
        endPos = transform.position;
        endPos.y = transform.position.y + linearLimit;

        //Get the Joint.
        configJoint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame.
    void Update()
    {
        //Debug.Log(initialPos + "" + endPos + "" + transform.position);
        
        //If the Joint's X position is less than 25% of it's min, invoke min methods.
        //This is due to configurable joints moving back a lot more than forward.
        if (ConfigJoint_Obj.transform.position.x <= (initialPos.x / 4))
        {
            OnMinLimitReached.Invoke();
        }

        //If the Joint's X position is more than its initial X, invoke max methods.
        else if (ConfigJoint_Obj.transform.position.x > initialPos.x)
        {
            OnMaxLimitReached.Invoke();
        }

        //If neither, return.
        else
        {
            return;
        }
    }

    //Stop moving the object by locking all the motion elements.
    //This will be invoked using the methods above.
    public void stopMoving()
    {
        configJoint.xMotion = ConfigurableJointMotion.Locked;
        configJoint.yMotion = ConfigurableJointMotion.Locked;
        configJoint.zMotion = ConfigurableJointMotion.Locked;

        ConfigJoint_Obj.transform.position = endPos;
    }
}
