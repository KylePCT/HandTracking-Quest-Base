using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandTrackingGrab : OVRGrabber
{
    //Variables.
    private OVRHand hand;
    public float pinchThreshold = 0.7f;

    protected override void Start()
    {
        //Prevents any existing behaviour affecting later code.
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame.
    public override void Update()
    {
        base.Update();
        CheckPinch();
    }

    void CheckPinch()
    {
        float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        //If there is no grabbed object and the pinch strength is > than the threshold and there are more than one grabbable candidates.
        if (!m_grabbedObj && pinchStrength > pinchThreshold && m_grabCandidates.Count > 0)
        {
            GrabBegin();
        }

        //If there is a grabbed object and the strength is no longer more than the threshold, let go.
        else if (m_grabbedObj && !(pinchStrength > pinchThreshold))
        {
            GrabEnd();
        }
    }

    //When the object is let go, allow it to stay in position.
    protected override void GrabEnd()
    {
        if (m_grabbedObj)
        {
            Vector3 linearVelocity = (transform.parent.position - m_lastPos) / Time.fixedDeltaTime;
            Vector3 angularVelocity = (transform.parent.eulerAngles - m_lastRot.eulerAngles) / Time.fixedDeltaTime;

            GrabbableRelease(linearVelocity, angularVelocity);
        }
        GrabVolumeEnable(true);
    }
}
