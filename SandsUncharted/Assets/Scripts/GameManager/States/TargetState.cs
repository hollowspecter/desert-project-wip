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
    private string interactButton = "A";
    [SerializeField]
    private string leftHandButton = "X";
    [SerializeField]
    private string targetTriggerAxis = "Target";
    [SerializeField]
    private float leftTriggerThreshold = 0.01f;
    [SerializeField]
    private string walkX = "Horizontal";
    [SerializeField]
    private string walkY = "Vertical";
    #endregion

    #region Properties (public)

    #endregion
    public static event InputAxisHandler Walk;

    public static event InputActionHandler LeftHand;
    public static event InputActionHandler OnTargetEnter;
    public static event InputActionHandler OnTargetExit;

    public static event InputInteractionHandler Interact;

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        float leftTrigger = Input.GetAxis(targetTriggerAxis);

        /* Input Handling */
        float xAxis = Input.GetAxis(walkX);
        float yAxis = Input.GetAxis(walkY);
        Walk(xAxis, yAxis);

        if (Input.GetButtonDown(interactButton)) {
            if (Interact != null)
                if (Interact())
                    stateMachine.ChangeToState(StateNames.InteractionState);
        }

        if (Input.GetButtonDown(leftHandButton)) {
            if (LeftHand != null)
                LeftHand();
        }

        /* State Changes */
        if (leftTrigger < leftTriggerThreshold) {
            stateMachine.ChangeToState(StateNames.BehindBackState);
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
        OnTargetEnter();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Target State");
        OnTargetExit();
    }
    #endregion
}