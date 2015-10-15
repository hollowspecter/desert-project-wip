using UnityEngine;
using System.Collections;

public class CharacterLogic : MonoBehaviour
{
    #region Variables (private)
    //Inspector serialized
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float directionDampTime = .25f; //delays animatior settings for a natural feel, goodfor pivoting
    [SerializeField]
    private ThirdPersonCam gamecam;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;


    //private global
    private float speed = 0.0f;
    private float direction = 0f;
    private float horizontal = 0.0f;
    private float vertical = 0.0f;
    private AnimatorStateInfo stateInfo;
    private float lookWeight = 0f;
    private Vector3 lookAt = new Vector3(0, 0, 0);

    //Hashes
    private int m_LocomotionId = 0;
    #endregion

    //Properties

    public Animator Animator { get { return this.anim; } }
    public float Speed { get { return this.speed; } }
    public float LocomotionThreshold { get { return 0.2f; } }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim.layerCount >= 2) {
            anim.SetLayerWeight(1, 1);
        }

        //Hash all animation names for perforcmance
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
    }

    // Update is called once per frame
    void Update()
    {
        if (anim && gamecam.CamState != ThirdPersonCam.CamStates.FirstPerson) {
            //Set animation stateInfo
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            //Pull values from controller/keyboard
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            //speed = new Vector2(horizontal, vertical).sqrMagnitude; OLD

            //Translate controls stick coordinatesinto world/cam/character space
            StickoWorldspace(this.transform, gamecam.transform, ref direction, ref speed);

            anim.SetFloat("Speed", speed);
            anim.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        //Rotate character model if stick is tilted right of left,
        //but only if character is moving in that direction
        if (IsInLocomotion() && ((direction >= 0 && horizontal >= 0) || direction < 0 && horizontal < 0)) {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, //lerping from zero rotation
                new Vector3(0f, rotationDegreePerSecond * (horizontal < 0f ? -1f : 1f), 0f), //shifting left or right
                Mathf.Abs(horizontal)); //based on the amount of horizontal value

            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(lookWeight);
        anim.SetLookAtPosition(lookAt);
    }

    public void setLookVars(Vector3 target, float weight)
    {
        lookWeight = weight;
        lookAt = target;
    }

    public void StickoWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirecttion = new Vector3(horizontal, 0, vertical);

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

        angleRootToMove /= 180f; //make it a hemisphere

        directionOut = angleRootToMove * directionSpeed;

    }

    public bool IsInLocomotion()
    {
        return stateInfo.nameHash == m_LocomotionId;
    }
}