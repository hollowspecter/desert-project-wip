///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class QuitState : State
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

        }
    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Quit State");
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Quit State");
    }
    #endregion
}