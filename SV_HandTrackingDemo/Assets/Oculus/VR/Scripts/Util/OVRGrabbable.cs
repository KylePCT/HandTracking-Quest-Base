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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// An object that can be grabbed and thrown by OVRGrabber.
/// </summary>
public class OVRGrabbable : MonoBehaviour
{
    [SerializeField]
    protected bool m_allowOffhandGrab = true;
    [SerializeField]
    protected bool m_snapPosition = false;
    [SerializeField]
    protected bool m_snapOrientation = false;
    [SerializeField]
    protected Transform m_snapOffset;
    [SerializeField]
    protected Collider[] m_grabPoints = null;
    [SerializeField]
    private bool isKinematic = true;

    protected bool m_grabbedKinematic = false;
    protected Collider m_grabbedCollider = null;
    protected OVRGrabber m_grabbedBy = null;

    [Header("Snap Objects to Snap Points")]
    public bool isSnappable;
    public GameObject ghostGuide;
    public Material ghostMat;
    private bool guideEntered = false;

    public UnityEvent objsWithTasks;
    public bool isTaskComplete;

    /// <summary>
    /// If true, the object can currently be grabbed.
    /// </summary>
    public bool allowOffhandGrab
    {
        get { return m_allowOffhandGrab; }
    }

	/// <summary>
	/// If true, the object is currently grabbed.
	/// </summary>
    public bool isGrabbed
    {
        get { return m_grabbedBy != null; }
    }

	/// <summary>
	/// If true, the object's position will snap to match snapOffset when grabbed.
	/// </summary>
    public bool snapPosition
    {
        get { return m_snapPosition; }
    }

	/// <summary>
	/// If true, the object's orientation will snap to match snapOffset when grabbed.
	/// </summary>
    public bool snapOrientation
    {
        get { return m_snapOrientation; }
    }

	/// <summary>
	/// An offset relative to the OVRGrabber where this object can snap when grabbed.
	/// </summary>
    public Transform snapOffset
    {
        get { return m_snapOffset; }
    }

	/// <summary>
	/// Returns the OVRGrabber currently grabbing this object.
	/// </summary>
    public OVRGrabber grabbedBy
    {
        get { return m_grabbedBy; }
    }

	/// <summary>
	/// The transform at which this object was grabbed.
	/// </summary>
    public Transform grabbedTransform
    {
        get { return m_grabbedCollider.transform; }
    }

	/// <summary>
	/// The Rigidbody of the collider that was used to grab this object.
	/// </summary>
    public Rigidbody grabbedRigidbody
    {
        get { return m_grabbedCollider.attachedRigidbody; }
    }

	/// <summary>
	/// The contact point(s) where the object was grabbed.
	/// </summary>
    public Collider[] grabPoints
    {
        get { return m_grabPoints; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ghostGuide)
        {
            guideEntered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ghostGuide)
        {
            guideEntered = false;
        }
    }

    /// <summary>
    /// Notifies the object that it has been grabbed.
    /// </summary>
    virtual public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        resetGrabPoints();
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;

        if (isKinematic)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (isSnappable)
        {
            ghostGuide.SetActive(true);
        }
    }

	/// <summary>
	/// Notifies the object that it has been released.
	/// </summary>
	virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        rb.velocity = linearVelocity;
        rb.angularVelocity = angularVelocity;
        m_grabbedBy = null;

        if (isSnappable)
        {
            if (guideEntered && isTaskComplete == false)
            {
                transform.position = ghostGuide.transform.position;
                transform.rotation = ghostGuide.transform.rotation;

                TaskComplete();
                Destroy(GetComponent<OVRGrabbable>());
                Destroy(ghostGuide);
            }

            ghostGuide.SetActive(false);
        }
    }

    void Awake()
    {
        if (m_grabPoints.Length == 0)
        {
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
				throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Collider[1] { collider };
        }
    }

    protected virtual void Start()
    {
        m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;

        if (isSnappable == true)
        {
            ghostGuide.GetComponent<Renderer>().material = ghostMat;
            ghostGuide.SetActive(false);

            guideEntered = false;
        }
    }

    void OnDestroy()
    {
        if (m_grabbedBy != null)
        {
            // Notify the hand to release destroyed grabbables
            m_grabbedBy.ForceRelease(this);
        }
    }

    public void resetGrabPoints()
    {
        if (m_grabPoints.Length == 0)
        {
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
                throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Collider[1] { collider };
        }
    }

    public void TaskComplete()
    {
        isTaskComplete = true;
        resetGrabPoints();
        objsWithTasks.Invoke();
    }
}
