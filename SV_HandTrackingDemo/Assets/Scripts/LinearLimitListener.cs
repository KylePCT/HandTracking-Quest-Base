using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinearLimitListener : MonoBehaviour
{
    public GameObject ConfigJoint_Obj;
    private ConfigurableJoint configJoint;

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
        initialPos = transform.position;

        linearLimit = GetComponent<ConfigurableJoint>().linearLimit.limit;

        endPos = transform.position;
        endPos.y = transform.position.y + linearLimit;

        configJoint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(initialPos + "" + endPos + "" + transform.position);

        if (ConfigJoint_Obj.transform.position.x <= (initialPos.x / 4))
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

    public void stopMoving()
    {
        configJoint.xMotion = ConfigurableJointMotion.Locked;
        configJoint.yMotion = ConfigurableJointMotion.Locked;
        configJoint.zMotion = ConfigurableJointMotion.Locked;

        ConfigJoint_Obj.transform.position = endPos;
    }
}
