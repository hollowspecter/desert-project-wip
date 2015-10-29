///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// State that is the Main Game State
/// </summary>
public class InGame1State : State
{
    #region variables (private)
    private ControlManager Control;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    protected override void Initialise()
    {
        Control = ControlManager.Instance();
    }

    public override void UpdateActive(double deltaTime)
    {
        if (Control.getButtonADown("InGame1")) {
            stateMachine.ChangeToState("InGame2");
        }
    }

    #endregion

    #region Methods
    
    public override void EnterState()
    {
        Debug.Log("Entered InGame1 State");
    }

    public override void ExitState()
    {
        Debug.Log("Exited InGame1 State");
    }
    #endregion


}