///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class CharacterLantern : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private Transform handSlot;
    [SerializeField]
    private Transform beltSlot;
    [SerializeField]
    private float movementSpeed = 1f;

    private LightSwitch lightSwitch;
    private bool usingLantern = false;
    private Transform handTarget;
    private Transform lantern;
    private Animator animator;
    private bool lanternGrabbed = false;
    #endregion

    #region Properties (public)
    public bool UsingLantern { get { return usingLantern; } }
    #endregion

    #region Unity event functions

    void Awake()
    {
        animator = GetComponent<Animator>();

        lantern = beltSlot.GetChild(0);
        if (lantern == null)
            Debug.LogError("Lantern reference not found", this);

        handTarget = transform.Find("LanternHandTarget");

        lightSwitch = lantern.GetComponentInChildren<LightSwitch>();
    }

    void ToggleLantern()
    {
        if (!usingLantern) {
            lantern.parent = handSlot;
            lantern.localPosition = Vector3.zero;
            lantern.localRotation = Quaternion.identity;
            lantern.GetComponentInChildren<CharacterJoint>().connectedBody = handSlot.GetComponent<Rigidbody>();
        }
        else {
            lantern.parent = beltSlot;
            lantern.localPosition = Vector3.zero;
            lantern.localRotation = Quaternion.identity;
            lantern.GetComponentInChildren<CharacterJoint>().connectedBody = beltSlot.GetComponent<Rigidbody>();

        }
        usingLantern = !usingLantern;
        lightSwitch.SwitchLight(usingLantern);
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator) {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (usingLantern) {
                // Set the right hand target position and rotation, if one has been assigned
                if (handTarget != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, .6f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, .9f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, handTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, handTarget.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }   
    #endregion

    #region Methods

    #endregion

    #region InputState Machine Methods

    void OnEnable()
    {
        BehindBackState.LeftHand += ToggleLantern;
    }

    void OnDisable()
    {
        BehindBackState.LeftHand -= ToggleLantern;

    }

    #endregion
}