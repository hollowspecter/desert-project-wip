using UnityEngine;
using System.Collections;

public class NotebookState : State
{
    #region variables (private)
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
    private string rightStickX = "RightStickX";
    [SerializeField]
    private string putBack = "Back";
    [SerializeField]
    private float rightStickThreshold = 0.1f;
    #endregion

    #region Properties (public)
    public static event InputAxisHandler MoveCursor;

    public static event InputActionHandler Draw;
    public static event InputActionHandler Erase;
    public static event InputActionHandler Undo;
    public static event InputActionHandler Redo;
    public static event InputActionHandler Clear;
    public static event InputActionHandler LiftedPen;

    public static event InputInteractionHandler PutNotebookBack;

    public static event InputActionHandler OnNotebookEnter;
    public static event InputActionHandler OnNotebookExit;
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Input Handling */
        float moveX = Input.GetAxis(moveCursorX);
        float moveY = Input.GetAxis(moveCursorY);
        if (MoveCursor != null)
            MoveCursor(moveX, moveY);

        if (Input.GetButton(drawButton)) {
            if (Draw != null) Draw();
        }
        if (Input.GetButton(eraseButton)) {
            if (Erase != null) Erase();
        }
        if (Input.GetButtonDown(clearButton)) {
            if (Clear != null) Clear();
        }
        if (Input.GetButtonDown(undoButton)) {
            if (Undo != null) Undo();
        }
        if (Input.GetButtonDown(redoButton)) {
            if (Redo != null) Redo();
        }
        if (Input.GetButtonUp(drawButton) || Input.GetButtonUp(eraseButton))
            LiftedPen();

        /* State Changing */

        // If you put away your notebook, go back to the state you have previously been
        if (Input.GetButtonDown(putBack)) {
            if (PutNotebookBack != null) PutNotebookBack();
            stateMachine.ChangeToState(stateMachine.PreviousState);
        }

        // if you want to switch to the right..
        float rightX = Input.GetAxis(rightStickX);
        if (rightX > rightStickThreshold) {
            //.. you can only switch, if you have NOT been in the behind the back state before...
            if (stateMachine.PreviousState != StateNames.BehindBackState) {
                //... else, switch to the previous
                stateMachine.ChangeToState(stateMachine.PreviousState);
            }
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        OnNotebookEnter();
        Debug.Log("Entered Notebook State");
    }

    public override void ExitState()
    {
        OnNotebookExit();
        Debug.Log("Exited Notebook State");
    }
    #endregion
}
