///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    }
    
    void Start()
    {
        StateMachine inGameStateMachine = transform.GetComponentInChildren<InGameStateMachine>();
		State behindBackState = GetComponentInChildren<BehindBackState>();
		State firstPersonState = GetComponentInChildren<FirstPersonState>();
		State targetState = GetComponentInChildren<TargetState>();
		State drawState = GetComponentInChildren<MapState>();
        State pauseState = GetComponentInChildren<PauseState>();
        State interactionState = GetComponentInChildren<InteractionState>();
        State notebookState = GetComponentInChildren<NotebookState>();
        State inventoryState = GetComponentInChildren<InventoryState>();
    
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
        interactionState.setParentStateMachine(inGameStateMachine);
        notebookState.setParentStateMachine(inGameStateMachine);
        inventoryState.setParentStateMachine(inGameStateMachine);
		
		inGameStateMachine.AddState(StateNames.BehindBackState, behindBackState);
		inGameStateMachine.AddState(StateNames.FirstPersonState, firstPersonState);
		inGameStateMachine.AddState(StateNames.TargetState, targetState);
        inGameStateMachine.AddState(StateNames.MapState, drawState);
        inGameStateMachine.AddState(StateNames.InteractionState, interactionState);
        inGameStateMachine.AddState(StateNames.NotebookState, notebookState);
        inGameStateMachine.AddState(StateNames.InventoryState, inventoryState);
		
		//add them all to all states DEPRECATED
		allStates.Add(StateNames.InGameStateMachine, inGameStateMachine);
		allStates.Add(StateNames.PauseState, pauseState);
		allStates.Add(StateNames.BehindBackState, behindBackState);
		allStates.Add(StateNames.TargetState, targetState);
        allStates.Add(StateNames.MapState, drawState);
        allStates.Add(StateNames.InteractionState, interactionState);
        allStates.Add(StateNames.NotebookState, notebookState);
        allStates.Add(StateNames.InventoryState, inventoryState);
		
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