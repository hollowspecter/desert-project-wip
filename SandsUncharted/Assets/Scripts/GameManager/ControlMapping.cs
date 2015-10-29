///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// Simple Container that holds the names (strings) of the corresponding buttons
/// and axis on our XBOX360 Controller. You can create a new ControlMapping Object
/// by RightClicking in the Editor.
/// Then fill in the strings.
/// Those strings will be passed to the Input Manager, so match the names there.
/// </summary>
public class ControlMapping : ScriptableObject
{
    // Trigger
    public string leftX;
    public string leftY;
    public string rightX;
    public string rightY;
    public string trigger;
    public string dpadX;
    public string dpadY;

    //Buttons
    public string buttonA;
    public string buttonB;
    public string buttonX;
    public string buttonY;
    public string buttonStart;
    public string buttonBack;
    public string buttonLeftShoulder;
    public string buttonRightShoulder;
    public string buttonLeftStick;
    public string buttonRightStick;
}