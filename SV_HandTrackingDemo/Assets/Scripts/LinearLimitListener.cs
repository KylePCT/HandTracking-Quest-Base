using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearLimitListener : MonoBehaviour
{
    public GameObject ConfigJoint_Obj;

    private Vector3 initialPos;
    private Vector3 endPos;
    private float linearLimit;

    //Event called on min reached
    public UnityEvent OnMinLimitReached;
    //Event called on max reached
    public UnityEvent OnMaxLimitReached;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = ConfigJoint_Obj.transform.position;

        linearLimit = ConfigJoint_Obj.GetComponent<ConfigurableJoint>().linearLimit.limit;

        endPos = ConfigJoint_Obj.transform.position;
        endPos.y = ConfigJoint_Obj.transform.position.y + linearLimit;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(initialPos + "" + endPos + "" + transform.position);

        if (ConfigJoint_Obj.transform.position.x <= initialPos.x)
        {
            OnMinLimitReached.Invoke();
        }

        else if (ConfigJoint_Obj.transform.position.x > initialPos.x)
        {
            OnMaxLimitReached.Invoke();
        }

        else
        {
            return;
        }
    }
}
