///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class GroundCheck : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float raycastLength = 1f;
    [SerializeField]
    private float sqrMaxDistanceToGround = 0.05f;
    [SerializeField]
    private float sqrMinDistanceToGround = 0.02f;
    [SerializeField]
    private float slopeAngle;
    [SerializeField]
    private float sqrDistance;

    private Animator animator;
    private bool grounded = false;
    #endregion

    #region Properties (public)
    public bool Grounded { get { return grounded; } }
    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        animator = transform.parent.GetComponent<Animator>();

        if (animator == null) {
            Debug.LogError("Groundcheck did not find the Animator Component in its parent, that is supposed to be the Character itself.", this);
        }
    }

    void Update()
    {
        RaycastHit hitPoint;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitPoint, raycastLength)){
            // check if the distance to the floor hitpoint is close enough
            // depending on the slope (higher slope, more distance allowed)
            slopeAngle = Vector3.Angle(Vector3.up, hitPoint.normal);
            sqrDistance = (transform.position - hitPoint.point).sqrMagnitude;

            float percentage = slopeAngle / 90f;
            float threshold = Mathf.Lerp(sqrMinDistanceToGround, sqrMaxDistanceToGround, percentage);
            grounded = sqrDistance <= threshold;
        }


        animator.SetBool("Grounded", grounded);
    }

    #endregion

    #region Methods

    #endregion
}