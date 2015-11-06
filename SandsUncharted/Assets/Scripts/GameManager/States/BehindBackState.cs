///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class BehindBackState : State
{
    #region variables (private)
    [SerializeField]
    private string walkX = "Horizontal";
    [SerializeField]
    private string walkY = "Vertical";
    [SerializeField]
    private string interactButton = "A";
    [SerializeField]
    private string leftHandButton = "X";
    [SerializeField]
    private string drawModeButton = "Y";
    [SerializeField]
    private string targetTriggerAxis = "Target";
    [SerializeField]
    private float leftTriggerThreshold = 0.01f;
    [SerializeField]
    private float firstPersonThreshold = 0.1f;

    private CharacterMovement character;
    #endregion

    #region Properties (public)
    public static event InputAxisHandler Walk;
    public static event InputActionHandler LeftHand;
    public static event InputActionHandler OnBehindBackEnter;
    public static event InputActionHandler OnBehindBackExit;

    public static event InputInteractionHandler Interact;

    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /*
         * Input Handling
         */

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

        /*
         * State Changing
         */

        if (Input.GetButtonDown(drawModeButton)) {
            stateMachine.ChangeToState(StateNames.DrawState);
        }

        float leftTrigger = Input.GetAxis(targetTriggerAxis);
        if (leftTrigger > leftTriggerThreshold) {
            stateMachine.ChangeToState(StateNames.TargetState);
        }

        float rightY = Input.GetAxis("RightStickY");
        if (rightY > firstPersonThreshold && !character.isMoving()) {
            //Debug.Log("right Y: " + rightY + "; threshold: " + firstPersonThreshold);
            stateMachine.ChangeToState(StateNames.FirstPersonState);
        }
    }

    protected override void Initialise()
    {
        character = GameObject.FindWithTag("Player").GetComponent<CharacterMovement>();
    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Behind Back State");
        OnBehindBackEnter();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Behind Back State");
        OnBehindBackExit();
    }
    #endregion
}