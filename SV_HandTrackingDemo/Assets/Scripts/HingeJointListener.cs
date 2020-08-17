using UnityEngine;
using UnityEngine.Events;

public class HingeJointListener : MonoBehaviour
{
    public bool isHingeJoint;

    //angle threshold to trigger if we reached limit
    public float angleBetweenThreshold = 1f;

    //State of the hinge joint : either reached min or max or none if in between
    public HingeJointState hingeJointState = HingeJointState.None;

    //Event called on min reached
    public UnityEvent OnMinLimitReached;
    //Event called on max reached
    public UnityEvent OnMaxLimitReached;

    public enum HingeJointState { Min, Max, None }
    private HingeJoint hinge;

    // Start is called before the first frame update
    void Start()
    {
        //Get the hinge component of the object.
        hinge = GetComponent<HingeJoint>();
    }

    private void FixedUpdate()
    {
        //Calculate max/min angles.
        float angleWithMinLimit = Mathf.Abs(hinge.angle - hinge.limits.min);
        float angleWithMaxLimit = Mathf.Abs(hinge.angle - hinge.limits.max);

        //Reached Min value.
        if (angleWithMinLimit < angleBetweenThreshold)
        {
            //If the state enters the min threshold, invoke.
            if (hingeJointState != HingeJointState.Min)
            {
                OnMinLimitReached.Invoke();
                hingeJointState = HingeJointState.Min;
            }
        }
        //Reached Max value.
        else if (angleWithMaxLimit < angleBetweenThreshold)
        {
            if (hingeJointState != HingeJointState.Max)
            {
                //When entering the max threshold, invoke.
                OnMaxLimitReached.Invoke();
                hingeJointState = HingeJointState.Max;
            }
        }

        //No limit reached.
        else
        {
            hingeJointState = HingeJointState.None;
        }
    }
}