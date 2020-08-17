using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Struct for class without function.
[System.Serializable]
public struct Gesture
{
    //Allows each gesture to have a name, data and an invokeable event.
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognised;
}

public class GestureDetection : MonoBehaviour
{

    //Help text.
    [Header("Enable Debug Mode to add Gestures using <G>.", order = 0)]
    [Help("Make sure you are in <PLAY MODE>!", UnityEditor.MessageType.Error)]
    public bool debugMode = true;

    [Space(-10, order = 2)]
    [Header("- Once all gestures are recorded, <RIGHT-CLICK> the inspector", order = 3)]
    [Space(-10, order = 4)]
    [Header("and click <COPY COMPONENTS>, turn off <PLAY MODE> and paste it", order = 5)]
    [Space(-10, order = 6)]
    [Header("back into the same script.", order = 7)]
    [Space(10, order = 8)]

    [Header("Variables:", order = 9)]
    public float threshold = 0.1f;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBones;
    private Gesture previousGesture;

    // Start is called before the first frame update.
    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    // Update is called once per frame.
    void Update()
    {
        //Set new fingerBones if it hasn't already been done in Start();
        if (fingerBones.Count == 0)
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
        }

        //If the Debug Mode is active and the user presses <G>, the current Hand Position is set as a new gesture.
        if (debugMode && Input.GetKeyUp(KeyCode.G))
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);

            //Save the gesture data.
            Save();
            Debug.Log("<color=green><b>New gesture saved.</b></color>");
        }

        //Check for new gesture.
        Gesture currentGesture = Recognise();
        bool hasRecognised = !currentGesture.Equals(new Gesture());

        //If the gesture is recognised as a saved gesture which isn't the same as the previous.
        if (hasRecognised && !currentGesture.Equals(previousGesture))
        {
            //Recognised a gesture
            Debug.Log("<color=cyan><b>Gesture recognised: </b></color>" + currentGesture.name);
            previousGesture = currentGesture;
            currentGesture.onRecognised.Invoke();
        }
    }

    //Save the gesture data.
    void Save()
    {
        //Populate the struct data with these.
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBones)
        {
            //Compare finger position relative to root.
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        } 

        //Populate the data.
        g.fingerDatas = data;
        gestures.Add(g);
    }

    //If a gesture is performed and is recognised as a saved gesture.
    Gesture Recognise()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        //Search the data and check if the finger positions match the current set by the user.
        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);

                //If the gesture is cancelled.
                if(distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            //If it is not cancelled, recognise the gesture.
            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        //Return the gesture found.
        return currentGesture;
    }
}
