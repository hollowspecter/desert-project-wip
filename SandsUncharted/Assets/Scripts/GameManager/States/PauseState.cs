///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class PauseState : State
{
    #region variables (private)
    [SerializeField]
    private string unpauseButton = "Start";
    #endregion

    #region Properties (public)
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        if (Input.GetButtonDown(unpauseButton)) {
            stateMachine.ChangeToState("InGameStateMachine");
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Pause State");
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Pause State");
    }
    #endregion
}