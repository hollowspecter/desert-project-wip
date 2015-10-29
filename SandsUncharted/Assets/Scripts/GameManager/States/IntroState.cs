///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// A Test State. Is the very first gamestate
/// </summary>
public class IntroState : State
{
    #region variables (private)

    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    protected override void Initialise()
    {

    }

    private float timer = 0;
    protected override void UpdateActive()
    {
        Debug.Log("Updating Intro State");
        timer += Time.deltaTime;
        if (timer > 1f)
            gameManager.ChangeToState(GameState.InGame);
    }

    #endregion

    #region Methods
    public override void EnterState()
    {
        Debug.Log("Entered Intro State");
    }

    public override void ExitState()
    {
        Debug.Log("Exited Intro State");
    }
    #endregion


}