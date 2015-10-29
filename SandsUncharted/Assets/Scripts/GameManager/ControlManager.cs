///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// The ControlManager
/// SINGLETON (Only one Instance is allowed)
/// 
/// Handles the different Control Mapping Objects and
/// manages the Passes to the Input Manager.
/// 
/// HOW TO USE:
/// Whenever an Object wants to get Input, you have to
/// specify the GameState it will be used in. Then get
/// a reference to an Instance of the Control Manager and
/// use the Input Methods from there, passing in the state.
/// 
/// The Manager checks, if the current state is active, and
/// if yes, allows you to retrieve that Input.
/// </summary>
public class ControlManager : MonoBehaviour
{
    #region singleton
    // singleton
    private static ControlManager controlManager;
    public static ControlManager Instance()
    {
        if (!controlManager) {
            controlManager = FindObjectOfType(typeof(ControlManager)) as ControlManager;
            if (!controlManager)
                Debug.LogError("There needs to be one active GameManager on a GameObject in your scene.");
        }

        return controlManager;
    }
    #endregion

    #region variables (private)

    [SerializeField]
    private ControlMapping defaultControls; // Default mapping
    [SerializeField]
    private ControlMapping userControls; // User Mapping

    private ControlMapping currentControls;
    private GameManager gameManager;

    #endregion

    #region Properties (public)

    #endregion

    void Awake()
    {
        // Get a GameManager Instance
        gameManager = GameManager.Instance();

        // Set the default Controls
        currentControls = defaultControls;
    }

    #region Methods

    /// <summary>
    /// Swap Controls between Default Control Mapping and User-Modifiable Control Mapping
    /// </summary>
    /// <param name="b">If true, the defaultControls will be used. Otherwise the users.</param>
    public void UseDefaultControls(bool b)
    {
        if (b)
            currentControls = defaultControls;
        else
            currentControls = userControls;
    }

    /*
     * Input Wrapping Methods
     */

    /* TRIGGER */
    public float getLeftX(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.leftX) : 0; }
    public float getLeftY(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.leftY) : 0; }
    public float getRightX(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.rightX) : 0; }
    public float getRightY(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.rightY) : 0; }
    public float getTrigger(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.trigger) : 0; }
    public float getDpadX(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.dpadX) : 0; }
    public float getDpadY(GameState state) { return gameManager.IsActive(state) ? Input.GetAxis(currentControls.dpadY) : 0; }

    /* BUTTONS */

    //ButtonA
    public bool getButtonA(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonA) : false; }
    public bool getButtonADown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonA) : false; }
    public bool getButtonAUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonA) : false; }

    //ButtonB
    public bool getButtonB(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonB) : false; }
    public bool getButtonBDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonB) : false; }
    public bool getButtonBUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonB) : false; }

    //ButtonX
    public bool getButtonX(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonX) : false; }
    public bool getButtonXDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonX) : false; }
    public bool getButtonXUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonX) : false; }

    //ButtonY
    public bool getButtonY(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonY) : false; }
    public bool getButtonYDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonY) : false; }
    public bool getButtonYUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonY) : false; }

    //Button Start
    public bool getButtonStart(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonStart) : false; }
    public bool getButtonStartDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonStart) : false; }
    public bool getButtonStartUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonStart) : false; }

    //Button Back
    public bool getButtonBack(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonBack) : false; }
    public bool getButtonBackDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonBack) : false; }
    public bool getButtonBackUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonBack) : false; }

    //Button Left Shoulder
    public bool getButtonLeftShoulder(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonLeftShoulder) : false; }
    public bool getButtonLeftShoulderDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonLeftShoulder) : false; }
    public bool getButtonLeftShoulderUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonLeftShoulder) : false; }

    //Button Right Shoulder
    public bool getButtonRightShoulder(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonRightShoulder) : false; }
    public bool getButtonRightShoulderDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonRightShoulder) : false; }
    public bool getButtonRightShoulderUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonRightShoulder) : false; }

    //Button Left Stick
    public bool getButtonLeftStick(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonLeftStick) : false; }
    public bool getButtonLeftStickDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonLeftStick) : false; }
    public bool getButtonLeftStickUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonLeftStick) : false; }

    //Button Right Stick
    public bool getButtonRightStick(GameState state) { return gameManager.IsActive(state) ? Input.GetButton(currentControls.buttonRightStick) : false; }
    public bool getButtonRightStickDown(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonDown(currentControls.buttonRightStick) : false; }
    public bool getButtonRightStickUp(GameState state) { return gameManager.IsActive(state) ? Input.GetButtonUp(currentControls.buttonRightStick) : false; }
    #endregion
}