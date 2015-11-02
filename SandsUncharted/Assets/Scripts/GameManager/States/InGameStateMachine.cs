///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class InGameStateMachine : StateMachine
{
    [SerializeField]
    private string pauseButton = "Start";

    public override void UpdateActive(double deltaTime)
    {
        base.UpdateActive(deltaTime);
        Debug.Log("Call derived UpdateActive from State Machine In Game");

        // Trigger Pause
        if (Input.GetButtonDown(pauseButton)) {
            stateMachine.ChangeToState("Pause");
        }
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered In Game Machine");
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exited In GameState Machine");
    }
}