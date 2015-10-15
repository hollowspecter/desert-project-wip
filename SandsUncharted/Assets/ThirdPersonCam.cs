///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>
///TODO: change Target behaviour so the camera doesnt follow the character when holding!

using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script for third person controller logic
/// </summary>
[RequireComponent (typeof (BarsEffect))]
public class ThirdPersonCam : MonoBehaviour
{
    #region variables (private)
    //Serialized private
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Transform followTransform;
    [SerializeField]
    private float distanceFromWall = .5f;
    [SerializeField]
    private float wideScreen = .2f;
    [SerializeField]
    private float targetingTime = .5f;


    //smoothing and damping
    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;


    //private global only
    private Vector3 lookDir;
    private Vector3 targetPosition;
    private BarsEffect barEffect;
    private CamStates camState = CamStates.Behind;
    private float targetTriggerTreshhold = 0.01f;

    #endregion

    #region Properties (public)

    public enum CamStates
    {
        Behind,
        FirstPerson,
        Target,
        Free
    }

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {

    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {
        followTransform = GameObject.FindWithTag("Player").transform;
        lookDir = followTransform.forward;

        barEffect = GetComponent<BarsEffect>();
        if (barEffect == null) {
            Debug.LogError("Attach a widescreenBarsEffect script to the camera.", this);
        }
    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {

    }

    // good for Camera stuff
    void LateUpdate()
    {
        Vector3 characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);

        //Determine camera state
        Debug.Log(Input.GetAxis("Target"));

        if (Input.GetAxis("Target") > targetTriggerTreshhold) {
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, wideScreen, targetingTime);
            camState = CamStates.Target;
        }
        else {
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);
            camState = CamStates.Behind;
        }

        switch (camState) {
            case CamStates.Behind:
                //Calculate direction from camera to player, kill Y, normalize to give valid direction with unit magnitude
                lookDir = characterOffset - this.transform.position;
                lookDir.y = 0f;
                lookDir.Normalize();

                //setting the target position to be the correct offset from the target
                targetPosition = characterOffset + followTransform.up * distanceUp - lookDir * distanceAway;
                break;
            case CamStates.Target:
                lookDir = followTransform.forward;
                break;
        }

        targetPosition = characterOffset + followTransform.up * distanceUp - lookDir * distanceAway;


        CompensateForWalls(characterOffset, ref targetPosition);

        //making smooth transition between it's current pos and the pos it want to be in
        SmoothPosition(this.transform.position, targetPosition);

        //make sure the cam is looking the rightway
        transform.LookAt(followTransform);
    }

    #endregion

    #region Methods

    private void SmoothPosition(Vector3 fromPos, Vector3 toPos)
    {
        //Making a smooth transition between camera's current position and the target position
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
    {
#if UNITY_EDITOR
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
#endif
        //Compensate for walls between camera
        RaycastHit wallHit = new RaycastHit();
        if (Physics.Linecast(fromObject, toTarget, out wallHit)) {
#if UNITY_EDITOR
            Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
#endif
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z) + wallHit.normal * distanceFromWall;
        }
    }

    #endregion
}