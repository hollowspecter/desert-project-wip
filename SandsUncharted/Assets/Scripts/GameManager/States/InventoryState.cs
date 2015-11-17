///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class InventoryState : State
{
    #region variables (private)
    [SerializeField]
    private string menuButton = "LB";

    #endregion

    #region Properties (public)
    public static event InputActionHandler OnEnter;
    public static event InputActionHandler OnExit;
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Input Handling */

        /* State Changing */
        if (Input.GetButtonUp(menuButton)) {
            stateMachine.ChangeToState(stateMachine.PreviousState);
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Inventory State");
        if (OnEnter != null) OnEnter();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Inventory State");
        if (OnExit != null) OnExit();
    }
    #endregion
}