/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Master SDK License Version 1.0 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/oculusmastersdk-1.0/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

// Code edited by Kyle Tugwell of Sonovision UK.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows grabbing and throwing of objects with the OVRGrabbable component on them.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OVRGrabber : MonoBehaviour
{
    // Grip trigger thresholds for picking up objects, with some hysteresis.
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;
    bool alreadyUpdated = false;

    private GameObject fingerTip; //Fingertip collision.

    // Demonstrates parenting the held object to the hand's transform when grabbed.
    // When false, the grabbed object is moved every FixedUpdate using MovePosition.
    // Note that MovePosition is required for proper physics simulation. If you set this to true, you can
    // easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
    // tower and noting a complete loss of friction.
    [SerializeField]
    protected bool m_parentHeldObject = false;

	// If true, this script will move the hand to the transform specified by m_parentTransform, using MovePosition in
	// FixedUpdate. This allows correct physics behavior, at the cost of some latency. In this usage scenario, you
	// should NOT parent the hand to the hand anchor.
	// (If m_moveHandPosition is false, this script will NOT update the game object's position.
	// The hand gameObject can simply be attached to the hand anchor, which updates position in LateUpdate,
    // gaining us a few ms of reduced latency.)
    [SerializeField]
    protected bool m_moveHandPosition = false;

    // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
    // Also used for ranking grab targets in case of multiple candidates.
    [SerializeField]
    protected Transform m_gripTransform = null;

    // Child/attached Colliders to detect candidate grabbable objects.
    [SerializeField]
    protected Collider[] m_grabVolumes = null;

    // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    protected OVRInput.Controller m_controller;

	// You can set this explicitly in the inspector if you're using m_moveHandPosition.
	// Otherwise, you should typically leave this null and simply parent the hand to the hand anchor
	// in your scene, using Unity's inspector.
    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    protected GameObject m_player;

	protected bool m_grabVolumeEnabled = true; //Can the object track collisions?
    protected Vector3 m_lastPos; //Last position.
    protected Quaternion m_lastRot; //Last rotation.
    protected Quaternion m_anchorOffsetRotation; //Offset rotation.
    protected Vector3 m_anchorOffsetPosition; //Offset position.
    protected float m_prevFlex; //Updates values from inputs.
	protected OVRGrabbable m_grabbedObj = null; //Tracks the grabbable i.e. is something grabbed?
    protected Vector3 m_grabbedObjectPosOff; //Grabbed object position offset.
    protected Quaternion m_grabbedObjectRotOff; //Grabbed object rotation offset.
	protected Dictionary<OVRGrabbable, int> m_grabCandidates = new Dictionary<OVRGrabbable, int>(); //Grabbable candidates.
	protected bool m_operatingWithoutOVRCameraRig = true; //Does the rig use the OVRCameraRig?

    public OVRGrabbable scriptGrabbable; //Grabbable script.

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public OVRGrabbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

    //Force release the object.
	public void ForceRelease(OVRGrabbable grabbable)
    {
        //canRelease = true when there is a grabbable 'grabbed' object.
        bool canRelease = ((m_grabbedObj != null) && (m_grabbedObj == grabbable));

        //If can release, let go using GrabEnd() method.
        if (canRelease)
        {
            GrabEnd();
        }
    }

    protected virtual void Awake()
    {
        //Set the offset position and rotation to the local Position and Rotation of the grabber.
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        //If 'Move Hand Position' isn't true, use the camera rig.
        if(!m_moveHandPosition)
        {
		    // If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.
		    OVRCameraRig rig = transform.GetComponentInParent<OVRCameraRig>();

            //If the rig is used, set and update the anchors.
		    if (rig != null)
		    {
			    rig.UpdatedAnchors += (r) => {OnUpdatedAnchors();};
			    m_operatingWithoutOVRCameraRig = false; //Future check.
		    }
        }
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if(m_parentTransform == null)
        {
			m_parentTransform = gameObject.transform;
        }
		// We're going to setup the player collision to ignore the hand collision.
		SetPlayerIgnoreCollision(gameObject, true);
    }

    virtual public void Update()
    {

        alreadyUpdated = false;
    }

    virtual public void FixedUpdate()
	{
        //If not using anchors...
		if (m_operatingWithoutOVRCameraRig)
        {
		    OnUpdatedAnchors();
        }
	}

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on
    // your hands or held objects, you may wish to switch to parenting.
    void OnUpdatedAnchors()
    {
        // Don't want to MovePosition multiple times in a frame, as it causes high judder in conjunction
        // with the hand position prediction in the runtime.
        if (alreadyUpdated) return;
        alreadyUpdated = true;

        //Set the position and rotation 
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;

        //If the hand is moved, set the rigidbody to have the same position and rotation.
        if (m_moveHandPosition)
        {
            GetComponent<Rigidbody>().MovePosition(destPos);
            GetComponent<Rigidbody>().MoveRotation(destRot);
        }

        //If the object is not parented, move the grabbed object dependant on the destinaation parameters.
        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }

        //Set the variables to the current position for future reference.
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        //Save flex value.
		float prevFlex = m_prevFlex;

		// Update values from inputs
		m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

		CheckForGrabOrRelease(prevFlex);
    }

    void OnDestroy()
    {
        //If the object is destroyed, run GrabEnd() to reset the correct parameters.
        if (m_grabbedObj != null)
        {
            GrabEnd();
        }
    }

    //Upon entering the collision of an object.
    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger.
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Add the grabbable.
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;


    }

    //Upon exiting the collision of an object.
    void OnTriggerExit(Collider otherCollider)
    {
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Remove the grabbable.
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        //If it is set to more than 1, reduce it by 1.
        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        //If it is 1 or less, remove the grabbable.
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    //Check if the object is being grabbed or not dependant on the flex value.
    protected void CheckForGrabOrRelease(float prevFlex)
    {
        //If the flex is between certain elements, designate a grab. 
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            GrabBegin();
        }

        //If the flex is not those values then designate it as not grabbed.
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            GrabEnd();
        }
    }

    protected virtual void GrabBegin()
    {
        //Collider values.
        float closestMagSq = float.MaxValue;
		OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate.
		foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            //canGrab true/false is defined by if there is no grabbable element in either hand.
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);

            //If you can't grab, reset the grab points and continue.
            if (!canGrab)
            {
                scriptGrabbable.resetGrabPoints();
                continue;
            }

            //Iterate the grabPoints and set colliders.
            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                
                // Store the closest grabbable.
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;

                //If the grabMagSq is less than the closestMagSq, set it to be grabbable.
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps.
        GrabVolumeEnable(false);

        //If there is a grabbable...
        if (closestGrabbable != null)
        {
            //If the grabbable element isGrabbed, set the closest grab to the hand.
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            //Set the grabbed object to the closestGrab and begin the grab.
            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            //Make sure to set the position and rotation to the current elements.
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if(m_grabbedObj.snapPosition)
            {
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = -m_grabbedObj.snapOffset.localPosition;
                    Vector3 snapOffsetScale = m_grabbedObj.snapOffset.lossyScale;
                    snapOffset = new Vector3(snapOffset.x * snapOffsetScale.x, snapOffset.y * snapOffsetScale.y, snapOffset.z * snapOffsetScale.z);

                    //If the controller is the left hand, reverse the offset.x value.
                    if (m_controller == OVRInput.Controller.LTouch)
                    {
                        snapOffset.x = -snapOffset.x;
                    }
                    m_grabbedObjectPosOff = snapOffset;
                }

                //If there is no offset, set the vector to zero.
                else
                {
                    m_grabbedObjectPosOff = Vector3.zero;
                }
            }

            //Set the position between the grabber and the grabbed object.
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }


            //If the object will snap orientation...
            if (m_grabbedObj.snapOrientation)
            {
                //Set the offset.
                if (m_grabbedObj.snapOffset)
                {
                    //Inverse the rotation if it needs it.
                    m_grabbedObjectRotOff = Quaternion.Inverse(m_grabbedObj.snapOffset.localRotation);
                }
                else
                {
                    //Set normal rotation.
                    m_grabbedObjectRotOff = Quaternion.identity;
                }
            }

            //If not snapping orientation...
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            SetPlayerIgnoreCollision(m_grabbedObj.gameObject, true);

            //If the object is set to be parented upon grab.
            if (m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
    {
        //If there is no grabbed object, do nothing and return.
        if (m_grabbedObj == null)
        {
            return;
        }

        //Set rigidbody and position/rotation values.
        Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

        //If forceTeleport is true, set the Rigidbody to the grabbable's transforms.
        if (forceTeleport)
        {
            grabbedRigidbody.transform.position = grabbablePosition;
            grabbedRigidbody.transform.rotation = grabbableRotation;
        }
        //If not, move the Rigidbody to the transforms.
        else
        {
            grabbedRigidbody.MovePosition(grabbablePosition);
            grabbedRigidbody.MoveRotation(grabbableRotation);
        }
    }

    virtual protected void GrabEnd()
    {
        //If an object is grabbed...
        if (m_grabbedObj != null)
        {
            //Let go of the grabbable.
            OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
            OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
            localPose = localPose * offsetPose;

            //Set velocity for the object.
            OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
            Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
            Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);
            GrabbableRelease(linearVelocity, angularVelocity);

            //Remove parent and 'grabbed' values.
            m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;

        }
        // Re-enable grab volumes to allow overlap events.
        GrabVolumeEnable(true);
    }

    //On release...
    protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        //Set the velocity.
        m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);

        //Remove parent.
        if(m_parentHeldObject) m_grabbedObj.transform.parent = null;

        //Re-enable collision.
        SetPlayerIgnoreCollision(m_grabbedObj.gameObject, false);

        //Set 'grabbed object' save to null.
        m_grabbedObj = null;
    }

    protected virtual void GrabVolumeEnable(bool enabled)
    {
        //If the volume is already enabled, do nothing and return.
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;

        //For the grab volumes, set colliders.
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        //If the grab volume is not enabled, clear the grab candidates.
        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }

	protected virtual void OffhandGrabbed(OVRGrabbable grabbable)
    {
        //If the obhect is grabbable by the offhand, set it to grabbable.
        if (m_grabbedObj == grabbable)
        {
            //Release with no motion.
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }

	protected void SetPlayerIgnoreCollision(GameObject grabbable, bool ignore)
	{
        //If there is a player...
		if (m_player != null)
		{
            //Get the colliders from the children.
			Collider[] playerColliders = m_player.GetComponentsInChildren<Collider>();

            //Using all the colliders, set the collision to ignore.
			foreach (Collider pc in playerColliders)
			{
				Collider[] colliders = grabbable.GetComponentsInChildren<Collider>();
				foreach (Collider c in colliders)
				{
					Physics.IgnoreCollision(c, pc, ignore);
				}
			}
		}
	}
}

