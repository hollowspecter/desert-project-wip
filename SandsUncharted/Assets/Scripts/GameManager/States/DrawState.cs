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
    private string undoButton = "leftShoulder";
    [SerializeField]
    private string clearButton = "rightShoulder";
    [SerializeField]
    private string moveCursorX = "Horizontal";
    [SerializeField]
    private string moveCursorY = "Vertical";
    #endregion

    #region Properties (public)
    public static event InputAxisHandler MoveCursor;

    public static event InputActionHandler Draw;
    public static event InputActionHandler Erase;
    public static event InputActionHandler Undo;
    public static event InputActionHandler Clear;
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /* Input Handling */
        float moveX = Input.GetAxis(moveCursorX);
        float moveY = Input.GetAxis(moveCursorY);
        MoveCursor(moveX, moveY);

        if (Input.GetButtonDown(drawButton)) Draw();
        if (Input.GetButtonDown(eraseButton)) Erase();
        if (Input.GetButtonDown(undoButton)) Undo();
        if (Input.GetButtonDown(clearButton)) Clear();

        /* State Changing */
        if (Input.GetButtonDown(backButton)) {
            stateMachine.ChangeToState("BehindBack");
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
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited Draw State");
    }
    #endregion
}