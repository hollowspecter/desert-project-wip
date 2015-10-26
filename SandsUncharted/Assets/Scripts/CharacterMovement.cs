///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class CharacterMovement : MonoBehaviour
{
    #region variables (private)
    //private serialized
    [SerializeField]
    private float rotationSpeed = 1.5f;
    [SerializeField]
    private float speedMultuplier = 2f;
    [SerializeField]
    private float speedMaximum = 5f;
    [SerializeField]
    private float movementThreshold = 0.15f;

    //private
    private Animator _animator;
    private Transform _transform;
    private Rigidbody _rigidbody;
    private ThirdPersonCamera camera;
    private Transform cameraTransform;
    private float speed = 0f;
    private float leftX = 0f;
    private float leftY = 0f;
    private float angle = 0f;
    private float moveAmount = 0f;
    private AnimatorStateInfo stateInfo;
    private float lookWeight = 0f;
    private Vector3 lookAt = new Vector3(0, 0, 0);

    //damp vars
    private float moveAmountDampTime = 0.1f;
    private float rotateAmountDampTime = 0.1f;

    //hashes
    private int m_moveId = 0;
    private int m_idleId = 0;

    #endregion

    #region Properties (public)
    public Animator Animator { get { return this._animator; } }
    public float Speed { get { return this.speed; } }
    public float MovementThreshold { get { return movementThreshold; } }
    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) {
            Debug.LogError("Animator Component not found!", this);
        }
        if (_animator.layerCount >= 2) {
            _animator.SetLayerWeight(1, 1);
        }

        _transform = GetComponent<Transform>();
        if (_transform == null) {
            Debug.LogError("Transform Component not found!", this);
        }

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null) {
            Debug.LogError("Rigidbody Component not found!", this);
        }
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCamera>();
        cameraTransform = camera.transform;

        // Hash all animation names for performance
        m_moveId = Animator.StringToHash("Base Layer.Move");
        m_idleId = Animator.StringToHash("Base Layer.Idle");
    }

    /// <summary>
    /// Update is called once per frame.
    /// Do not use for physics computations.
    /// </summary>
    void Update()
    {
        if (!_animator)
            return;

        // Set animation stateinfo
        stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // Pull values from joystick
        leftX = Input.GetAxis("Horizontal");
        leftY = Input.GetAxis("Vertical");

        // Reset speed and angle
        float charSpeed = 0f;
        float charAngle = 0f;

        // Translate controls joystick coords to camera space
        JoystickToWorldspace(_transform, cameraTransform, ref charSpeed, ref charAngle);

        // This is here, so you can modify it beforehand (pivots, running, sprinting, whatsoever)
        speed = charSpeed * speedMultuplier;
        speed = speed > speedMaximum ? speedMaximum : speed;
        angle = charAngle;

        speed = (camera.CamState == ThirdPersonCamera.CamStates.FirstPerson) ? 0f : speed;
        _animator.SetFloat("Speed", speed);
    }

    /// <summary>
    /// Fixed Update for Physics Calculations
    /// </summary>
    void FixedUpdate()
    {
        if (camera.CamState != ThirdPersonCamera.CamStates.FirstPerson) {
            //Rotate
            Quaternion shift = Quaternion.Euler(new Vector3(0, angle, 0));
            Quaternion targetRotation = _transform.rotation * shift;
            _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

            //Move forward
            Vector3 movement = _transform.forward * speed;
            movement.y = _rigidbody.velocity.y;
            _rigidbody.velocity = movement;
        }        
    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {

    }

    #endregion

    #region Methods
    public void JoystickToWorldspace(Transform root, Transform camera, ref float speedOut, ref float angleOut/*, ref Vector3 moveDirOut*/)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirection = new Vector3(leftX, 0, leftY);

        speedOut = stickDirection.sqrMagnitude;

        if (speedOut > movementThreshold) {
            // Get camera rotation
            Vector3 CameraDirection = camera.forward;
            CameraDirection.y = 0.0f; // kill Y
            Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(CameraDirection));

            // Convert joystick input in Worldspace coordinates
            Vector3 moveDir = referentialShift * stickDirection;
            Vector3 axisSign = Vector3.Cross(moveDir, rootDirection);

            //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDir, Color.green);
            //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
            //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
            //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisSign, Color.red);

            float angleRootToMove = Vector3.Angle(rootDirection, moveDir) * (axisSign.y >= 0 ? -1f : 1f);

            //angleRootToMove /= 180f;

            angleOut = angleRootToMove;

            if (speedOut < movementThreshold)
                speedOut = 0;
        }
        else {
            angleOut = 0;
            speedOut = 0;
        }
        

        //moveDirOut = moveDir;
    }	
    #endregion

    #region Animator methods
    public bool isMoving()
    {
        return stateInfo.fullPathHash == m_moveId;
    }

    public void setLookVars(Vector3 target, float weight)
    {
        lookWeight = weight;
        lookAt = target;
    }

    void OnAnimatorIK(int layerIndex)
    {
        _animator.SetLookAtWeight(lookWeight);
        _animator.SetLookAtPosition(lookAt);
    }
    #endregion


}