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
    public static event InputActionHandler Interact;
    public static event InputActionHandler LeftHand;
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
            Interact();
        }

        if (Input.GetButtonDown(leftHandButton)) {
            LeftHand();
        }

        /*
         * State Changing
         */

        if (Input.GetButtonDown(drawModeButton)) {
            stateMachine.ChangeToState("Draw");
        }

        float leftTrigger = Input.GetAxis(targetTriggerAxis);
        if (leftTrigger < leftTriggerThreshold) {
            stateMachine.ChangeToState("Target");
        }

        float rightY = Input.GetAxis("RightStickY");
        if (rightY > firstPersonThreshold && !character.isMoving()) {
            stateMachine.ChangeToState("FirstPerson");
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
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Behind Back State");
    }
    #endregion
}