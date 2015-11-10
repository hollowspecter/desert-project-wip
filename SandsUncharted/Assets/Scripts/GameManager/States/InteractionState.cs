///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class InteractionState : State
{
    #region variables (private)
    [SerializeField]
    private string InteractButton = "A";
    [SerializeField]
    private string toggleNotebookButton = "Back";
    [SerializeField]
    private string rightStickXAxis = "RightStickX";
    [SerializeField]
    private float rightStickThreshold = -0.1f;
    #endregion

    #region Properties (public)

    #endregion
    public static InputInteractionHandler InteractAndExit;
    public static InputActionHandler OnEnter;
    public static InputActionHandler OnExit;

    public static InputActionHandler CloseNotebook;

    public static event InputInteractionHandler SwitchToNotebook; // returns true if Notebook is open and ready to switch to
    public static event InputInteractionHandler ToggleNotebook; //should return true if you put it OUT
                                                                //and false, if you put it away
    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Interacting? */
        if (Input.GetButtonDown(InteractButton)) {
            if (InteractAndExit != null) {
                if (InteractAndExit()) { //Closing Interaction?
                    CloseNotebook();
                    stateMachine.ChangeToState(StateNames.BehindBackState);
                }

            }
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
        OnEnter();
        Debug.Log("Entered Interaction State");
    }

    public override void ExitState()
    {
        OnExit();
        Debug.Log("Exited Interaction State");
    }
    #endregion
}