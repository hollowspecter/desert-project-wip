///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// State that is the Main Game State
/// </summary>
public class InGameState : State
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

    protected override void UpdateActive()
    {

    }

    #endregion

    #region Methods
    
    public override void EnterState()
    {
        Debug.Log("Entered InGameState");
    }

    public override void ExitState()
    {
        Debug.Log("Exited InGame State");
    }
    #endregion


}