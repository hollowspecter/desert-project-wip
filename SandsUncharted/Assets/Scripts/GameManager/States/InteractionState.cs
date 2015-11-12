///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class InteractionState : State
{
    #region variables (private)
    [SerializeField]
    private string buttonA = "A";
    [SerializeField]
    private string buttonB = "B";
    [SerializeField]
    private string buttonY = "Y";
    [SerializeField]
    private string buttonX = "X";
    [SerializeField]
    private string buttonLB = "LB";
    [SerializeField]
    private string buttonRB = "RB";
    [SerializeField]
    private string leftX = "Horizontal";
    [SerializeField]
    private string leftY = "Vertical";
    [SerializeField]
    private string buttonBack = "Back";
    [SerializeField]
    private string triggersAxis = "Triggers";
    [SerializeField]
    private string triggerLAxis = "TriggerL";
    [SerializeField]
    private string triggerRAxis = "TriggerR";
    [SerializeField]
    private string rightX = "RightStickX";
    [SerializeField]
    private string rightY = "RightStickY";
    [SerializeField]
    private float rightStickThreshold = -0.1f;
    #endregion

    #region Properties (public)

    #endregion
    public static InputInteractionHandler InteractA;
    public static InputInteractionHandler InteractB;
    public static InputInteractionHandler InteractX;
    public static InputInteractionHandler InteractY;
    public static InputInteractionHandler InteractLB;
    public static InputInteractionHandler InteractRB;

    public static InputAxisHandler GetTriggers;
    public static InputAxisHandler GetTriggerLR;
    public static InputAxisHandler LeftStick;
    public static InputAxisHandler RightStick;

    public static InputActionHandler OnEnter;
    public static InputActionHandler OnExit;
    public static InputActionHandler OnLiftedPen;

    public static InputActionHandler buttonADown;

    public static InputActionHandler CloseNotebook;

    public static event InputInteractionHandler SwitchToNotebook; // returns true if Notebook is open and ready to switch to
    public static event InputInteractionHandler ToggleNotebook;   //should return true if you put it OUT
                                                                  //and false, if you put it away
    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Every button Action could be used
         * to close the Interaction by returning true
         */

        if (Input.GetButtonDown(buttonA)) {
            if (InteractA != null)
                if (InteractA())  //Closing Interaction?
                    Exit();
        }
        else if (Input.GetButton(buttonA))
            if (buttonADown != null)
                buttonADown();
        if (Input.GetButtonDown(buttonB))
            if (InteractB != null)
                if (InteractB())  //Closing Interaction?
                    Exit();
        if (Input.GetButtonDown(buttonX))
            if (InteractX != null)
                if (InteractX())  //Closing Interaction?
                    Exit();
        if (Input.GetButtonDown(buttonY))
            if (InteractY != null)
                if (InteractY())  //Closing Interaction?
                    Exit();
        if (Input.GetButtonDown(buttonLB))
            if (InteractLB != null)
                if (InteractLB())  //Closing Interaction?
                    Exit();
        if (Input.GetButtonDown(buttonRB))
            if (InteractRB != null)
                if (InteractRB())  //Closing Interaction?
                    Exit();

        float x, y;
        if (GetTriggers != null) {
            x = Input.GetAxis(triggersAxis);
            GetTriggers(x, 0);
        } 
        if (GetTriggerLR != null) {
            x = Input.GetAxis(triggerLAxis);
            y = Input.GetAxis(triggerRAxis);
            GetTriggerLR(x, y);
        } 
        if (LeftStick != null) {
            x = Input.GetAxis(leftX);
            y = Input.GetAxis(leftY);
            LeftStick(x, y);
        } 
        if (RightStick != null) {
            x = Input.GetAxis(rightX);
            y = Input.GetAxis(rightY);
            RightStick(x, y);
        } 

        //For drawing riddles
        if (Input.GetButtonUp(buttonA))
            if (OnLiftedPen != null)
                OnLiftedPen();

        /* Notebook Code */
        // if the back button is pressed..
        if (Input.GetButtonDown(buttonBack)) {
            if (ToggleNotebook != null) {
                // check, if you take out the notebook? If yes...
                if (ToggleNotebook()) {
                    //.. switch to the Notebook State
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }
        float rightXvalue = Input.GetAxis(rightX);
        if (rightXvalue < rightStickThreshold) {
            if (SwitchToNotebook != null) {
                if (SwitchToNotebook()) {
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }
    }

    protected override void Initialise()
    {

    }

    void Exit()
    {
        CloseNotebook();
        stateMachine.ChangeToState(StateNames.BehindBackState);
    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        OnEnter();
        Debug.Log("Entered Interaction State");
    }

    public override void ExitState()
    {
        OnExit();
        Debug.Log("Exited Interaction State");
    }
    #endregion
}