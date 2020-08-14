using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGuide : MonoBehaviour
{
    [SerializeField]
    [Help("Utilise this prefab to create more moveable objects. The Mesh child has the correct scripts needed for it to be interactable.", UnityEditor.MessageType.Info)]
    private GameObject Mesh;

    [SerializeField]
    [Help("The Offset child allows the Mesh to snap to the fingertip of your hand. Do not move unless absolutely necessary.", UnityEditor.MessageType.Info)]
    private GameObject Offset;

    [SerializeField]
    [Help("The Guide child is where you want the player to move the Mesh to. This will become invisible in-game until the player grabs the Mesh. When the Mesh object touches the Guide, it will snap to it when the player lets go of the Mesh.", UnityEditor.MessageType.Info)]
    private GameObject Guide;

    [SerializeField]
    [Help("Once you've configured your interactions, add the Mesh object into your MenuManager page where the task will be under 'Moveable Task Objects'.", UnityEditor.MessageType.Info)]
    private pageSystem MenuManager;

    [SerializeField]
    [Help("Lastly, select your Mesh again and add 'MenuManager' into the 'ObjsWithTasks' event inside of <OVRGrabbable> and select 'TaskManager > PieceFinished'.  Your interaction should be complete and count as a task for the set page.", UnityEditor.MessageType.Info)]
    private GameObject TaskFinishedMesh;
}
