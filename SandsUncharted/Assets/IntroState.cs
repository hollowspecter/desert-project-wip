///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class IntroState : State
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

    private float timer = 0;
    void Update()
    {
        if (Active) {
            Debug.Log("Updating Intro State");
            timer += Time.deltaTime;
            if (timer > 1f)
                gameManager.ChangeToState(GameState.InGame);
        }
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