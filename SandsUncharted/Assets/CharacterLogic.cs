using UnityEngine;
using System.Collections;

public class CharacterLogic : MonoBehaviour
{
    #region Variables (private)
    //Inspector serialized
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float directionDampTime = .25f; //delays animatior settings for a natural feel, goodfor pivoting
    [SerializeField]
    private float speedDampTime = .05f;
    [SerializeField]
    private ThirdPersonCam gamecam;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;
    [SerializeField]
    private float fovDampTime = .1f;
    [SerializeField]
    private float jumpMultiplier = 2.0f;
    [SerializeField]
    private CapsuleCollider _capCollider;
    [SerializeField]
    private float jumpDist = 1f;


    //private global
    private float speed = 0.0f;
    private float direction = 0f;
    private float charAngle = 0f;
    private float leftX = 0.0f;
    private float leftY = 0.0f;
    private AnimatorStateInfo stateInfo;
    private AnimatorTransitionInfo transInfo;
    private float lookWeight = 0f;
    private Vector3 lookAt = new Vector3(0, 0, 0);
    private float capsuleHeight;

    //private constants
    private const float SPRINT_SPEED = 2.0f;
    private const float SPRINT_FOV = 75f;
    private const float NORMAL_FOV = 60f;

    //Hashes
    private int m_LocomotionId = 0;
    private int m_LocomotionPivotLId = 0;
    private int m_LocomotionPivotRId = 0;
    private int m_LocomotionPivotLTransId = 0;
    private int m_LocomotionPivotRTransId = 0;
    private int m_LocomotionJumpId = 0;
    private int m_IdleJumpId = 0;
    #endregion

    //Properties
    public Animator Animator { get { return this._animator; } }
    public float Speed { get { return this.speed; } }
    public float LocomotionThreshold { get { return 0.2f; } }

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _capCollider = GetComponent<CapsuleCollider>();
        capsuleHeight = _capCollider.height;

        if (_animator.layerCount >= 2) {
            _animator.SetLayerWeight(1, 1);
        }

        //Hash all animation names for perforcmance
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
        m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
        m_LocomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
        m_LocomotionJumpId = Animator.StringToHash("Base Layer.LocomotionJump");
        m_IdleJumpId = Animator.StringToHash("Base Layer.IdleJump");
    }

    // Update is called once per frame
    void Update()
    {
        if (_animator && gamecam.CamState != ThirdPersonCam.CamStates.FirstPerson) {
            //Set animation stateInfo
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            transInfo = _animator.GetAnimatorTransitionInfo(0);

            //Press A to jump
            if (Input.GetButton("A") && !IsInJump()) {
                _animator.SetTrigger("Jump");
            }

            //Pull values from controller/keyboard
            leftX = Input.GetAxis("Horizontal");
            leftY = Input.GetAxis("Vertical");

            charAngle = 0f;
            direction = 0f;
            float charSpeed = 0f;

            //speed = new Vector2(horizontal, vertical).sqrMagnitude; OLD

            //Translate controls stick coordinatesinto world/cam/character space
            StickoWorldspace(this.transform, gamecam.transform, ref direction, ref charSpeed, ref charAngle, IsInPivot());

            //Press X to sprint
            if (Input.GetButton("X")) {
                speed = Mathf.Lerp(speed, SPRINT_SPEED, Time.deltaTime);
                gamecam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gamecam.GetComponent<Camera>().fieldOfView, SPRINT_FOV, fovDampTime * Time.deltaTime);
            }
            else {
                speed = charSpeed;
                gamecam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gamecam.GetComponent<Camera>().fieldOfView, NORMAL_FOV, fovDampTime * Time.deltaTime);
            }

            _animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);

            //damping makes turning around difficult
            _animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

            //only set angle when you are in a pivot
            if (speed > LocomotionThreshold) {
                if (!IsInPivot()) {
                    _animator.SetFloat("Angle", charAngle);
                }
            }

            if (speed < LocomotionThreshold && Mathf.Abs(leftX) < 0.05f) {  //Dead zone
                _animator.SetFloat("Direction", 0f);
                _animator.SetFloat("Angle", 0f);
            }
        }
    }

    void FixedUpdate()
    {
        //Rotate character model if stick is tilted right of left,
        //but only if character is moving in that direction
        if (IsInLocomotion() && ((direction >= 0 && leftX >= 0) || direction < 0 && leftX < 0)) {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, //lerping from zero rotation
                new Vector3(0f, rotationDegreePerSecond * (leftX < 0f ? -1f : 1f), 0f), //shifting left or right
                Mathf.Abs(leftX)); //based on the amount of horizontal value

            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }

        if (IsInJump()) {
            float oldY = transform.position.y;
            transform.Translate(Vector3.up * jumpMultiplier * Animator.GetFloat("JumpCurve"));
            if (IsInLocomotionJump()) {
                transform.Translate(Vector3.forward * Time.deltaTime * jumpDist);
            }
            _capCollider.height = capsuleHeight + (_animator.GetFloat("CapsuleCurve") * .5f);
            if (gamecam.CamState != ThirdPersonCam.CamStates.Free) {
                gamecam.ParentRig.Translate(Vector3.up * (transform.position.y - oldY));
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        _animator.SetLookAtWeight(lookWeight);
        _animator.SetLookAtPosition(lookAt);
    }

    public void setLookVars(Vector3 target, float weight)
    {
        lookWeight = weight;
        lookAt = target;
    }

    public void StickoWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirecttion = new Vector3(leftX, 0, leftY);

        speedOut = stickDirecttion.sqrMagnitude;

        //Get camera rotation
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f; //kill Y
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

        //Convert joystick input to Worldspace coordinates
        Vector3 moveDirection = referentialShift * stickDirecttion;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

#if UNITY_EDITOR
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirecttion, Color.blue);
#endif

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

        if (!isPivoting) {
            angleOut = angleRootToMove;
        }

        angleRootToMove /= 180f; //make it a hemisphere

        directionOut = angleRootToMove * directionSpeed;

    }

    public bool IsInLocomotion()
    {
        return stateInfo.fullPathHash == m_LocomotionId;
    }

    public bool IsInPivot()
    {
        return stateInfo.fullPathHash == m_LocomotionPivotLId ||
            stateInfo.fullPathHash == m_LocomotionPivotRId ||
            transInfo.fullPathHash == m_LocomotionPivotLTransId ||
            transInfo.fullPathHash == m_LocomotionPivotRTransId;
    }

    public bool IsInJump()
    {
        return IsInIdleJump() || IsInLocomotionJump();
    }

    public bool IsInIdleJump()
    {
        return stateInfo.fullPathHash == m_IdleJumpId;
    }

    public bool IsInLocomotionJump()
    {
        return stateInfo.fullPathHash == m_LocomotionJumpId;
    }
}