using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class DoorInteractable : Interactable
{
    [SerializeField]
    private Vector3 targetPosition;

    private ThirdPersonCamera cam;
    private DoorDrawingScript drawingScript;

    public override void Init()
    {
        interactionString = "Draw";
        cam = Camera.main.GetComponent<ThirdPersonCamera>();
        Assert.IsNotNull<ThirdPersonCamera>(cam);
        targetPosition = transform.position + transform.TransformVector(targetPosition);
        drawingScript = GetComponentInChildren<DoorDrawingScript>();
        Assert.IsNotNull<DoorDrawingScript>(drawingScript);
    }

    public override void Interact()
    {
        InteractionState.InteractB += DoneDrawing;//#1
        cam.OverrideTargetPosition = targetPosition;//#2: override the target position of the camera
        drawingScript.Activate();//#3: Activate the drawing script
    }

    bool DoneDrawing()
    {
        InteractionState.InteractB -= DoneDrawing;//#1
        cam.ResetOverrideTargetPosition();//reverse #2
        drawingScript.Deactivate();//reverse #3
        return true;
    }
}
