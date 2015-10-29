///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// State Class
/// Is an abstract class that must be inherited by new
/// States. Initialised the states and wraps up the Update
/// function.
/// </summary>
public abstract class State : MonoBehaviour
{
    protected StateMachine stateMachine; // parent state machine
    protected GameManager gameManager; // parent state machine

    [HideInInspector]
    public bool Active;

    public void setParentStateMachine(StateMachine s)
    {
        stateMachine = s;
    }

    /// <summary>
    /// Initialises the gameManager Instance Active field.
    /// DO NOT OVERRIDE
    /// </summary>
    void Awake()
    {
        Active = false;
        gameManager = GameManager.Instance();
        Initialise();
    }

   ///// <summary>
   ///// Wraps up the Update function.
   ///// Do not Override or use when derive from State
   ///// </summary>
   //void Update()
   //{
   //    if (Active) UpdateActive();
   //}

    /// <summary>
    /// Override this as your Update Function in the State.
    /// Is only called when the current state is active
    /// </summary>
    abstract public void UpdateActive(double deltaTime);

    /// <summary>
    /// Override this as your Awake/Start/Init Function
    /// </summary>
    abstract protected void Initialise();

    /// <summary>
    /// Gets called when entering this state.
    /// For more info see GameManager class
    /// </summary>
    abstract public void EnterState();

    /// <summary>
    /// Gets called when exitigng this state.
    /// For more info see GameManager class
    /// </summary>
    abstract public void ExitState();
}