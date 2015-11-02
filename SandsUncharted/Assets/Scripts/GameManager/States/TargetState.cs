///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class TargetState : State
{
    #region variables (private)
    [SerializeField]
    private string targetTriggerAxis = "Target";
    [SerializeField]
    private float leftTriggerThreshold = 0.01f;
    #endregion

    #region Properties (public)

    #endregion
    public static event InputActionHandler Targeting;
    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        float leftTrigger = Input.GetAxis(targetTriggerAxis);

        /* Input Handling */
        if (leftTrigger < leftTriggerThreshold) {
            Targeting();
        }
        /* State Changing */
        else {
            stateMachine.ChangeToState("BehindBack");
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Target State");
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Target State");
    }
    #endregion
}