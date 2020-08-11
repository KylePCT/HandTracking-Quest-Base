using UnityEngine;
using UnityEngine.Events;

public class HingeJointListener : MonoBehaviour
{
    //angle threshold to trigger if we reached limit
    public float angleBetweenThreshold = 1f;

    //State of the hinge joint : either reached min or max or none if in between
    public HingeJointState hingeJointState = HingeJointState.None;

    //Event called on min reached
    public UnityEvent OnMinLimitReached;
    //Event called on max reached
    public UnityEvent OnMaxLimitReached;

    public bool isHingeJoint;
    public bool isConfigurableJoint;

    public enum HingeJointState { Min, Max, None }
    private HingeJoint hinge;

    public enum ConfigJointState { Min, Max, None }
    private ConfigurableJoint configHinge;

    // Start is called before the first frame update
    void Start()
    {
        if (isHingeJoint)
        {
            hinge = GetComponent<HingeJoint>();
        }

        if (isConfigurableJoint)
        {
            configHinge = GetComponent<ConfigurableJoint>();
        }
    }

    private void FixedUpdate()
    {

        if (isHingeJoint)
        {
            float angleWithMinLimit = Mathf.Abs(hinge.angle - hinge.limits.min);
            float angleWithMaxLimit = Mathf.Abs(hinge.angle - hinge.limits.max);

            //Reached Min
            if (angleWithMinLimit < angleBetweenThreshold)
            {
                if (hingeJointState != HingeJointState.Min)
                    OnMinLimitReached.Invoke();

                hingeJointState = HingeJointState.Min;
            }
            //Reached Max
            else if (angleWithMaxLimit < angleBetweenThreshold)
            {
                if (hingeJointState != HingeJointState.Max)
                    OnMaxLimitReached.Invoke();

                hingeJointState = HingeJointState.Max;
            }
            //No Limit reached
            else
            {
                hingeJointState = HingeJointState.None;
            }
        }

        if (isConfigurableJoint)
        {

        }
    }
}