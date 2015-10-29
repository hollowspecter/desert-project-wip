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

        StateMachine ingameStateMachine = transform.GetChild(0).GetComponent<StateMachine>();
        State introState = GetComponentInChildren<IntroState>();
        State ingame1State = GetComponentInChildren<InGame1State>();
        State ingame2State = GetComponentInChildren<InGame2State>();
        State quitState = GetComponentInChildren<QuitState>();

        //layer 1
        ingameStateMachine.setParentStateMachine(stateMachine);
        introState.setParentStateMachine(stateMachine);
        quitState.setParentStateMachine(stateMachine);

        stateMachine.AddState("Intro", introState);
        stateMachine.AddState("InGame", ingameStateMachine);
        stateMachine.AddState("Quit", quitState);

        //layer 2
        ingame1State.setParentStateMachine(ingameStateMachine);
        ingame2State.setParentStateMachine(ingameStateMachine);

        ingameStateMachine.AddState("InGame1", ingame1State);
        ingameStateMachine.AddState("InGame2", ingame2State);

        //add them all to all states
        allStates.Add("Intro", introState);
        allStates.Add("InGame", ingameStateMachine);
        allStates.Add("Quit", quitState);
        allStates.Add("InGame1", ingame1State);
        allStates.Add("InGame2", ingame2State);

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