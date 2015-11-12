using UnityEngine;
using System.Collections;

public class DoorDrawingScript : DrawingScript
{
    private DoorRiddle riddle;
    private Texture2D tex;

    public void Activate()
    {
        InteractionState.LeftStick += ReceiveLeftStickInput;
        InteractionState.buttonADown += OnDraw;
        InteractionState.InteractX += ClearInteraction;
        InteractionState.OnLiftedPen += CheckTexture;
        InteractionState.OnLiftedPen += OnLiftedPen;
    }

    public void Deactivate()
    {
        InteractionState.LeftStick -= ReceiveLeftStickInput;
        InteractionState.buttonADown -= OnDraw;
        InteractionState.InteractX -= ClearInteraction;
        InteractionState.OnLiftedPen -= CheckTexture;
        InteractionState.OnLiftedPen -= OnLiftedPen;
    }

    private void CheckTexture()
    {
        riddle.CheckOneDoor(tex, this);
    }

    private bool ClearInteraction()
    {
        OnClear();
        return false;
    }

    protected override void OnOverrideEnable()
    {
        tex = (Texture2D)GetComponent<MeshRenderer>().material.GetTexture("_FrontDrawTex");
        riddle = transform.parent.parent.GetComponent<DoorRiddle>();   
    }

    protected override void OnOverrideDisable()
    {

    }
}
