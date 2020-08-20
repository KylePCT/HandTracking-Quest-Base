using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporting : MonoBehaviour
{
    private LineRenderer teleportLine;
    public GameObject fingerPoint;
    public GameObject player;
    private bool groundDetected;
    private Vector3 groundPos;
    public GameObject positionMarker;

    // Start is called before the first frame update
    void Start()
    {
        teleportLine = GetComponent<LineRenderer>();
        groundDetected = false;
        teleportLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        positionMarker.SetActive(groundDetected);
    }

    public void TeleportAim()
    {
        RaycastHit hit;
        Debug.Log("Aiming...");
        teleportLine.enabled = true;


        if (Physics.Raycast(fingerPoint.transform.position, fingerPoint.transform.forward, out hit))
        {
            Debug.Log(hit);

            teleportLine.SetPosition(1, new Vector3(0, 0, hit.distance));
            groundDetected = true;
            groundPos = hit.point;

            if (groundDetected)
            {
                positionMarker.transform.position = groundPos;
                positionMarker.transform.LookAt(groundPos);

                Debug.Log(groundPos);
            }
        }

        else
        {
            teleportLine.SetPosition(1, new Vector3(0, 0, 5000));
        }
    }

    public void Teleport()
    {
        if (groundDetected)
        {
            player.transform.position = groundPos;
            Debug.Log("Teleported");
        }
    }

    public void TeleportAimCancel()
    {
        teleportLine.enabled = false;
        Debug.Log("Teleport cancelled.");
    }
}
