using UnityEngine;
using System.Collections;

public class noteBookDrawingScript : DrawingScript
{
    
    protected override void OnOverrideEnable()
    {
        DrawState.MoveCursor += ReceiveLeftStickInput;
        DrawState.Draw += OnDraw;
        DrawState.Erase += OnErase;
        DrawState.OnDrawExit += OnDrawExit;
        DrawState.Clear += OnClear;
        DrawState.Undo += Undo;
        DrawState.Redo += Redo;
        DrawState.LiftedPen += OnLiftedPen;
    }

    protected override void OnOverrideDisable()
    {
        DrawState.MoveCursor -= ReceiveLeftStickInput;
        DrawState.Draw -= OnDraw;
        DrawState.Erase -= OnErase;
        DrawState.OnDrawExit -= OnDrawExit;
        DrawState.Clear -= OnClear;
        DrawState.Undo -= Undo;
        DrawState.Redo -= Redo;
        DrawState.LiftedPen -= OnLiftedPen;
    }
}
