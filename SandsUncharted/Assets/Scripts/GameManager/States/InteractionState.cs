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
    #endregion

    #region Properties (public)

    #endregion
    public static InputInteractionHandler InteractAndExit;
    public static InputActionHandler OnEnter;
    public static InputActionHandler OnExit;
    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Interacting? */
        if (Input.GetButtonDown(InteractButton)) {
            if (InteractAndExit != null) {
                Debug.Log("Interact And Exit Called");
                if (InteractAndExit()) { //Closing Interaction?
                    Debug.Log("Read and Close was true");
                    stateMachine.ChangeToState(StateNames.BehindBackState);
                }
                else {
                    Debug.Log("Read and Close was false");
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