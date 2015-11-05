///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class DrawState : State
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

    public static event InputActionHandler OnDrawEnter;
    public static event InputActionHandler OnDrawExit;
    #endregion

    #region Unity event functions

    void Update()
    {

    }

    public override void UpdateActive(double deltaTime)
    {
        /* Input Handling */
        float moveX = Input.GetAxis(moveCursorX);
        float moveY = Input.GetAxis(moveCursorY);
        if (MoveCursor != null)
            MoveCursor(moveX, moveY);

        float trigger = Input.GetAxis("Target");
        if (TurnMap != null)
            TurnMap(trigger, 0f);

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