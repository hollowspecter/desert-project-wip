///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class FirstPersonState : State
{
    #region variables (private)
    [SerializeField]
    private string lookX = "Horizontal";
    [SerializeField]
    private string lookY = "Vertical";
    [SerializeField]
    private string backButton = "B";
    [SerializeField]
    private string targetTriggerAxis = "Target";
    [SerializeField]
    private float leftTriggerThreshold = 0.01f;
    #endregion

    #region Properties (public)
    public static event InputAxisHandler lookAround;
    #endregion

    #region Unity event functions

    public override void UpdateActive(double deltaTime)
    {
        /*
         * Input Handling
         */

        float xAxis = Input.GetAxis(lookX);
        float yAxis = Input.GetAxis(lookY);
        lookAround(xAxis, yAxis);

        /*
         * State Changing
         */

        if (Input.GetButtonDown(backButton)) {
            stateMachine.ChangeToState("BehindBack");
        }

        float leftTrigger = Input.GetAxis(targetTriggerAxis);
        if (leftTrigger < leftTriggerThreshold) {
            stateMachine.ChangeToState("Target");
        }
    }

    protected override void Initialise()
    {

    }

    #endregion

    #region Methods

    public override void EnterState()
    {
        Debug.Log("Entered First Person State");
        Application.Quit();
    }

    public override void ExitState()
    {
        Debug.Log("Exited First Person State");
    }
    #endregion
}