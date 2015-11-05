///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The different states the game can be in.
/// I.e. Intro, Pause, MainMenu, Game, MiniGame, Quit...
/// The first State will be the first state to be entered
/// by the Game Manager
/// </summary>
public enum GameState { Intro, InGame, Quit, NUMOFSTATES };

/// <summary>
/// The Game Manager
/// SINGLETON - Only one instance is allowed
/// 
/// IDEALLY this Game Manager also handles loading
/// and switching scenes.
/// 
/// Manages the Game State Machine.
/// Here you need to initialise the different states.
/// </summary>

public class GameManager : MonoBehaviour
{
    #region singleton
    // singleton
    private static GameManager gameManager;
    public static GameManager Instance()
    {
        if (!gameManager) {
            gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
            if (!gameManager)
                Debug.LogError("There needs to be one active GameManager on a GameObject in your scene.");
        }

        return gameManager;
    }
    #endregion

    #region variables (private)
    private Dictionary<string, State> allStates;
    private StateMachine stateMachine;

    #endregion

    #region Properties (public)

    public GameState currentState { get; private set; } // current state the game is in

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        allStates = new Dictionary<string, State>();

        // Initialise the States and Statemachines
        stateMachine = GetComponent<StateMachine>();

        StateMachine inGameStateMachine = transform.GetComponentInChildren<InGameStateMachine>();
        State behindBackState = GetComponentInChildren<BehindBackState>();
        State firstPersonState = GetComponentInChildren<FirstPersonState>();
        State targetState = GetComponentInChildren<TargetState>();
        State drawState = GetComponentInChildren<DrawState>();
        State pauseState = GetComponentInChildren<PauseState>();

        //layer 1
        inGameStateMachine.setParentStateMachine(stateMachine);
        pauseState.setParentStateMachine(stateMachine);

        stateMachine.AddState(StateNames.InGameStateMachine, inGameStateMachine);
        stateMachine.AddState(StateNames.PauseState, pauseState);

        //layer 2
        behindBackState.setParentStateMachine(inGameStateMachine);
        firstPersonState.setParentStateMachine(inGameStateMachine);
        targetState.setParentStateMachine(inGameStateMachine);
        drawState.setParentStateMachine(inGameStateMachine);

        inGameStateMachine.AddState(StateNames.BehindBackState, behindBackState);
        inGameStateMachine.AddState(StateNames.FirstPersonState, firstPersonState);
        inGameStateMachine.AddState(StateNames.TargetState, targetState);
        inGameStateMachine.AddState(StateNames.DrawState, drawState);

        //add them all to all states DEPRECATED
        allStates.Add(StateNames.InGameStateMachine, inGameStateMachine);
        allStates.Add(StateNames.PauseState, pauseState);
        allStates.Add(StateNames.BehindBackState, behindBackState);
        allStates.Add(StateNames.TargetState, targetState);
        allStates.Add(StateNames.DrawState, drawState);

        stateMachine.EnterState();
    }

    void Update()
    {
        stateMachine.UpdateActive(Time.deltaTime);
    }

    #endregion

    #region Methods

    public bool IsActive(string state)
    {
        return allStates[state].Active;
    }
    #endregion
}