///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

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

    [SerializeField]
    private GameState currentStateDisplayed;

    private State[] states = new State[(int)GameState.NUMOFSTATES]; // all the GameStates

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
        // Initialise the States
        states[(int)GameState.Intro] = GetComponentInChildren<IntroState>();
        states[(int)GameState.InGame] = GetComponentInChildren<InGameState>();
        states[(int)GameState.Quit] = GetComponentInChildren<QuitState>();

        // Check if all the states have been initialised correctly
        for (int i=0; i<states.Length; ++i){
            if (states[i]==null){
                Debug.LogError("State "+i+" has not been initialised correctly. Please attach the State to the States Gameobject under GameManger");
                Application.Quit();
            }
        }

        // Enter the first State
        GameState defaultState = (GameState) 0;
        currentState = defaultState;
        UpdateDisplayedState();

        states[(int)currentState].EnterState();
        states[(int)currentState].Active = true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Calls the ExitState() method of the old State
    /// and the EnterState() method of the new State.
    /// Sets old state inactive and new one Active;
    /// </summary>
    /// <param name="gameState">GameState to change to</param>
    public void ChangeToState(GameState gameState)
    {
        states[(int)currentState].ExitState();
        states[(int)currentState].Active = false;
        states[(int)gameState].EnterState();
        states[(int)gameState].Active = true;
        currentState = gameState;

        UpdateDisplayedState();
    }

    /// <summary>
    /// Checks if the given state is currently Active
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool IsActive(GameState state)
    {
        return states[(int)state].Active;
    }

    /// <summary>
    /// Updates the State displayed in the Inspector
    /// </summary>
    private void UpdateDisplayedState()
    {
        currentStateDisplayed = currentState;
    }
    #endregion
}