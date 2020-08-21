using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporting : MonoBehaviour
{
    private LineRenderer teleportLine;
    public GameObject fingerPoint;
    public GameObject player;
    private bool groundDetected;
    private bool aiming;
    private Vector3 groundPos;
    public GameObject positionMarker;
    private int layerMask;

    [Header("Line Colour Customisation")]
    public Color defaultStartColour;
    public Color defaultEndColour;

    public Color hitStartColour;
    public Color hitEndColour;

    // Start is called before the first frame update
    void Start()
    {
        teleportLine = GetComponent<LineRenderer>();
        groundDetected = false;
        teleportLine.enabled = false;
        layerMask = 1 << 8;
        aiming = false;
    }

    void Update()
    {
        positionMarker.SetActive(groundDetected);

        if (aiming)
        {
            RaycastHit hit;

            teleportLine.enabled = true;

            if (Physics.Raycast(fingerPoint.transform.position, fingerPoint.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                teleportLine.startColor = hitStartColour;
                teleportLine.endColor = hitEndColour;
                teleportLine.SetPosition(1, new Vector3(0, 0, hit.distance));
                groundDetected = true;
                groundPos = hit.point;

                if (groundDetected)
                {
                    positionMarker.transform.position = groundPos;
                    positionMarker.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    Debug.Log("Teleport line hit " + groundPos + " at object: " + hit.collider.gameObject.name);
                }
            }

            else
            {
                teleportLine.startColor = defaultStartColour;
                teleportLine.endColor = defaultEndColour;
                teleportLine.SetPosition(1, new Vector3(0, 0, 5000));
            }
        }
    }

    public void TeleportAim()
    {
        Debug.Log("Aiming...");
        aiming = true;
    }

    public void Teleport()
    {
        if (groundDetected)
        {
            player.transform.position = groundPos;
            Debug.Log("Teleported");
            aiming = false;
        }
    }

    public void TeleportAimCancel()
    {
        teleportLine.enabled = false;
        Debug.Log("Teleport cancelled.");
        aiming = false;
    }
}
