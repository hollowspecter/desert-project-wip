///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class StateMachine : State
{
    [SerializeField]
    protected Dictionary<string, State> states;

    protected string currentState = "";
    protected string defaultState = "";

    public void ChangeToState(string newState)
    {
        // if we have a current state
        if (currentState.Length != 0) {
            states[currentState].ExitState();
            states[currentState].Active = false;
            currentState = "";
        }

        // if state machine does contain this state, make it the new one
        if (states.ContainsKey(newState)) {
            states[newState].EnterState();
            states[newState].Active = true;
            currentState = newState;
        }
        else {
            if (stateMachine != null) { // is there a layer above?
                stateMachine.ChangeToState(newState); // then go and look there
            }
            else {
                Debug.LogError("The state " + newState + " has not been found.");
            }
        }
    }

    public void AddState(string name, State state)
    {
        states.Add(name, state);

        if (defaultState.Length == 0) {
            defaultState = name;
        }
    }

    /* State Functions */

    public override void UpdateActive(double deltaTime)
    {
        Debug.Log("Called Base StateMachine Update Active");

        if (currentState.Length == 0) {
            ChangeToState(defaultState);
        }
        states[currentState].UpdateActive(deltaTime);
    }

    protected override void Initialise()
    {
        states = new Dictionary<string, State>();
    }

    public override void EnterState()
    {
        Debug.Log("Entered State Machine");
        Active = true;
        if (currentState.Length == 0) {
            ChangeToState(defaultState);
        }
    }

    public override void ExitState()
    {
        Active = false;

        // make that so the state does not change when reentering the statemachine
        if (currentState.Length > 0) {
            defaultState = currentState;
        }

        Debug.Log("Exited State Machine");
    }
}