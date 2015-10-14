///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script for third person controller logic
/// </summary>
public class ThirdPersonCam : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Transform follow;
    private Vector3 targetPosition;
    #endregion

    #region Properties (public)

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
        follow = GameObject.FindWithTag("Player").transform;
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
        //setting the target position to be the correct offset from the target
        targetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;
        Debug.DrawRay(follow.position, Vector3.up * distanceUp, Color.red);
        Debug.DrawRay(follow.position, -1f * follow.forward * distanceAway, Color.blue);
        Debug.DrawLine(follow.position, targetPosition, Color.magenta);

        //making smooth transition between it's current pos and the pos it want to be in
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

        //make sure the cam is looking the rightway
        transform.LookAt(follow);
    }

    #endregion

    #region Methods

    #endregion
}