///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// A State to shut down the game.
/// </summary>
public class StateTemplate : State
{
    #region variables (private)

    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {

    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Quit State");
    }

    public override void ExitState()
    {
        Debug.Log("Exited Quit State");
    }
    #endregion

}