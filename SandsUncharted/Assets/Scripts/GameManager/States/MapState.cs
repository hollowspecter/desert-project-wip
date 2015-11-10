///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class MapState : State
{
    #region variables (private)
    [SerializeField]
    private string backButton = "Y";
    [SerializeField]
    private string drawButton = "A";
    [SerializeField]
    private string eraseButton = "B";
    [SerializeField]
    private string undoButton = "LB";
    [SerializeField]
    private string redoButton = "RB";
    [SerializeField]
    private string clearButton = "X";
    [SerializeField]
    private string moveCursorX = "Horizontal";
    [SerializeField]
    private string moveCursorY = "Vertical";
    [SerializeField]
    private string triggerAxis = "Target";
    [SerializeField]
    private string toggleNotebookButton = "Back";
    [SerializeField]
    private string rightStickXAxis = "RightStickX";
    [SerializeField]
    private float rightStickThreshold = -0.1f;

    #endregion

    #region Properties (public)
    public static event InputAxisHandler MoveCursor;
    public static event InputAxisHandler TurnMap;

    public static event InputActionHandler Draw;
    public static event InputActionHandler Erase;
    public static event InputActionHandler Undo;
    public static event InputActionHandler Redo;
    public static event InputActionHandler Clear;
    public static event InputActionHandler LiftedPen;

    public static event InputInteractionHandler SwitchToNotebook; // returns true if Notebook is open and ready to switch to
    public static event InputInteractionHandler ToggleNotebook; //should return true if you put it OUT
                                                                //and false, if you put it away

    public static event InputActionHandler OnDrawEnter;
    public static event InputActionHandler OnDrawExit;
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Input Handling */
        float moveX = Input.GetAxis(moveCursorX);
        float moveY = Input.GetAxis(moveCursorY);
        if (MoveCursor != null)
            MoveCursor(moveX, moveY);

        if(TurnMap != null)
        {
            TurnMap(Input.GetAxis(triggerAxis), 0f);
        }
        if (Input.GetButton(drawButton))
        {
            if (Draw != null) Draw();
        }
        if (Input.GetButton(eraseButton))
        {
            if (Erase != null) Erase();
        }

        if (Input.GetButtonDown(clearButton))
        {
            if (Clear != null) Clear();
        }
        if (Input.GetButtonDown(undoButton))
        {
            if (Undo != null) Undo();
        }
        if (Input.GetButtonDown(redoButton))
        {
            if (Redo != null) Redo();
        }

        if (Input.GetButtonUp(drawButton) || Input.GetButtonUp(eraseButton))
            LiftedPen();

        /* Notebook Code */
        // if the back button is pressed..
        if (Input.GetButtonDown(toggleNotebookButton)) {
            if (ToggleNotebook != null) {
                // check, if you take out the notebook? If yes...
                if (ToggleNotebook()) {
                    //.. switch to the Notebook State
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }
        float rightX = Input.GetAxis(rightStickXAxis);
        if (rightX < rightStickThreshold) {
            if (SwitchToNotebook != null) {
                if (SwitchToNotebook()) {
                    stateMachine.ChangeToState(StateNames.NotebookState);
                }
            }
        }

        /* State Changing */
        if (Input.GetButtonDown(backButton))
        {
            stateMachine.ChangeToState(StateNames.BehindBackState);
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered Draw State");
        OnDrawEnter();
        //Debug.Log("redo:" + redoButton + " undo: " + undoButton);
    }

    public override void ExitState()
    {
        Debug.Log("Exited Draw State");
        OnDrawExit();
    }
    #endregion
}