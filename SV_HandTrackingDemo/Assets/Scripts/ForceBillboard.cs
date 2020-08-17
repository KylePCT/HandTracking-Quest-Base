using UnityEngine;

public class ForceBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        //Force the object to follow the camera.
        transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
    }
}