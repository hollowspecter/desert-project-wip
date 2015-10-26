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
    private bool ikActive = true;
    [SerializeField]
    private Transform leftHandObj;

    private Animator animator;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator) {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive) {
                // Set the right hand target position and rotation, if one has been assigned
                if (leftHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
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
}