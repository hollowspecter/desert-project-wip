///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

public enum GameState { Intro, InGame, Exit, NUMOFSTATES };

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
[RequireComponent (typeof(IntroState))]
[RequireComponent(typeof(InGameState))]
[RequireComponent(typeof(QuitState))]
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

    private State[] states = new State[(int)GameState.NUMOFSTATES];
    #endregion

    #region Properties (public)
    public GameState currentState { get; private set; }
    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        // Initialise the States
        states[(int)GameState.Intro] = GetComponent<IntroState>();
        states[(int)GameState.InGame] = GetComponent<InGameState>();
        states[(int)GameState.Exit] = GetComponent<QuitState>();

        // Check if all the states have been initialised correctly
        for (int i=0; i<states.Length; ++i){
            if (states[i]==null){
                Debug.LogError("State "+i+" has not been initialised correctly");
                Application.Quit();
            }
        }

        // Enter the first State
        GameState defaultState = (GameState) 0;
        currentState = defaultState;
        currentStateDisplayed = currentState;

        states[(int)currentState].EnterState();
        states[(int)currentState].Active = true;
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {

    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {

    }

    #endregion

    #region Methods
    public void ChangeToState(GameState gameState)
    {
        states[(int)currentState].ExitState();
        states[(int)currentState].Active = false;
        states[(int)gameState].EnterState();
        states[(int)gameState].Active = true;
        currentState = gameState;
    }
    #endregion
}