using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandTrackingGrab : OVRGrabber
{
    private OVRHand hand;
    private float pinchThreshold = 0.7f;

    protected override void Start()
    {
        //prevents any existing behaviour affecting later code
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        CheckPinch();
    }

    void CheckPinch()
    {
        float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        //if there is no grabbed object and the pinch strength is > than the threshold and there are more than one grabbable candidates
        if (!m_grabbedObj && pinchStrength > pinchThreshold && m_grabCandidates.Count > 0)
        {
            GrabBegin();
        }

        //if there is a grabbed object and the strength is no longer more than the threshold, let go
        else if (m_grabbedObj && !(pinchStrength > pinchThreshold))
        {
            GrabEnd();
        }
    }
}
