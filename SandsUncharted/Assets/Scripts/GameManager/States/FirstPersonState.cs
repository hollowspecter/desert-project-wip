///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class FirstPersonState : State
{
    #region variables (private)
    [SerializeField]
    private string lookX = "Horizontal";
    [SerializeField]
    private string lookY = "Vertical";
    [SerializeField]
    private string backButton = "B";
    [SerializeField]
    private string targetTriggerAxis = "Target";
    [SerializeField]
    private float leftTriggerThreshold = 0.01f;
    [SerializeField]
    private string toggleNotebookButton = "Back";
    [SerializeField]
    private string rightStickXAxis = "RightStickX";
    [SerializeField]
    private float rightStickThreshold = -0.1f;
    [SerializeField]
    private string menuButton = "LB";
    #endregion

    #region Properties (public)
    public static event InputAxisHandler lookAround;
    public static event InputActionHandler onFirstPersonEnter;
    public static event InputActionHandler onFirstPersonExit;

    public static event InputInteractionHandler SwitchToNotebook; // returns true if Notebook is open and ready to switch to
    public static event InputInteractionHandler ToggleNotebook; //should return true if you put it OUT
                                                                //and false, if you put it away
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /*
         * Input Handling
         */

        float xAxis = Input.GetAxis(lookX);
        float yAxis = Input.GetAxis(lookY);
        if (lookAround != null)
            lookAround(xAxis, yAxis);

        /*
         * State Changing
         */

        if (Input.GetButtonDown(backButton)) {
            stateMachine.ChangeToState(StateNames.BehindBackState);
        }

        float leftTrigger = Input.GetAxis(targetTriggerAxis);
        if (leftTrigger > leftTriggerThreshold) {
            stateMachine.ChangeToState(StateNames.TargetState);
        }

        if (Input.GetButtonDown(menuButton)) {
            stateMachine.ChangeToState(StateNames.InventoryState);
        }

        /* Notebook Code */
        // if the back button is pressed..
        if (Input.GetButtonDown(toggleNotebookButton)) {
            if (ToggleNotebook != null) {
                // check, if you take out the notebook? If yes...
                if (ToggleNotebook()) {
                    //.. switch to the Notebook State
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }
        float rightX = Input.GetAxis(rightStickXAxis);
        if (rightX < rightStickThreshold) {
            if (SwitchToNotebook != null) {
                if (SwitchToNotebook()) {
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }

        
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered First Person State");
        onFirstPersonEnter();
    }

    public override void ExitState()
    {
        Debug.Log("Exited First Person State");
        onFirstPersonExit();
    }
    #endregion
}