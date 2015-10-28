///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class InGameState : State
{
    #region variables (private)

    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        gameManager = GameManager.Instance();
        Active = false;
    }

    void Update()
    {
        if (Active) {
            if (Input.GetKeyUp(KeyCode.K)) {
                gameManager.ChangeToState(GameState.Exit);
            }
        }
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