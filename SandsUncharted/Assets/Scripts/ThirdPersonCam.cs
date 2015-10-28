///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>
///TODO: change Target behaviour so the camera doesnt follow the character when holding!

using UnityEngine;
using System.Collections;

struct CameraPosition
{
    //position to align camera to, probably somewhere behind the character
    //or position to point camera at, probably somewhere along
    //character's axis
    private Vector3 pos;
    //transform used for any rotation
    private Transform transform;

    public Vector3 Position { get { return pos; } set { pos = value; } }
    public Transform Transform { get { return transform; } set { transform = value; } }

    public void Init(string camName, Vector3 pos, Transform transform, Transform parent)
    {
        this.pos = pos;
        this.transform = transform;
        this.transform.name = camName;
        this.transform.parent = parent;
        this.transform.localPosition = Vector3.zero;
        transform.localPosition = this.pos;
    }
}

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
    private float distanceAwayMultiplier = 1.5f;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float distanceUpMultiplier = 5f;
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
    [SerializeField]
    private Vector3 firstPersonViewPosition = new Vector3(0.0f, 1.6f, 0.2f);
    [SerializeField]
    private CharacterLogic characterLogic;
    [SerializeField]
    private float firstPersonLookSpeed = 1.5f;
    [SerializeField]
    private Vector2 firstPersonXAxisClamp = new Vector2(-70.0f, 90.0f);
    [SerializeField]
    private float fPSRotationDegreePerSecond = 120f;
    [SerializeField]
    private float freeThreshold = 0.5f;
    [SerializeField]
    private float fpsThreshold = -0.5f;
    [SerializeField]
    private Vector2 camMinDistFromChar = new Vector2(1f, -.5f);
    [SerializeField]
    private float rightStickThreshold = 0.1f;
    [SerializeField]
    private const float freeRotationDegreePerSecond = -5f;



    //smoothing and damping
    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    private Vector3 velocityLookDir = Vector3.zero;
    [SerializeField]
    private float lookDirDampTime = 0.1f;


    //private global only
    private Vector3 lookDir;
    private Vector3 currLookDir;
    private Vector3 targetPosition;
    private BarsEffect barEffect;
    private CamStates camState = CamStates.Behind;
    private CameraPosition firstPersonCamPos;
    private float xAxisRot = 0.0f;
    private float lookWeight;
    private Vector3 headLookAt = new Vector3(0f, 0f, 0f);
    private Vector3 savedRigToGoal;
    private float distanceAwayFree;
    private float distanceUpFree;
    private Vector2 rightStickPrevFrame = Vector2.zero;
    private Transform parentRig;
    private Vector3 characterOffset;
    private float lastStickMin = float.PositiveInfinity; //used to prevent from zooming in when holding back on rightstick

    //private constants
    private const float TARGET_TRIGGER_TRESHHOLD = 0.01f;


    #endregion

    #region Properties (public)

    public CamStates CamState { get { return this.camState; } }

    public enum CamStates
    {
        Behind,
        FirstPerson,
        Target,
        Free,
		Map
    }

    public Vector3 RigToGoalDirection
    {
        get
        {
            // Move height and distance from character in separate parentRig transform since RotateAround has control of both position and rotation
            Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
            // Can't calculate distanceAway from a vector with Y axis rotation in it; zero it out
            rigToGoalDirection.y = 0f;

            return rigToGoalDirection;
        }
    }

    public Transform ParentRig { get { return parentRig; } }

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
        //characterLogic = followTransform.parent.GetComponent<CharacterLogic>();
        if (characterLogic == null) {
            Debug.LogError("Reference from ThirdPersonCam to CharacterLogic failed.", this);
        }
        lookDir = followTransform.forward;
        currLookDir = followTransform.forward;

        barEffect = GetComponent<BarsEffect>();
        if (barEffect == null) {
            Debug.LogError("Attach a widescreenBarsEffect script to the camera.", this);
        }

        //position and parent a GO where first person view should be
        firstPersonCamPos = new CameraPosition();
        firstPersonCamPos.Init(
            "First Person Camera",
            firstPersonViewPosition,
            new GameObject().transform,
            followTransform
            );

        parentRig = transform.parent;

        // Intialize values to avoid having 0s
        characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);
        distanceUpFree = distanceUp;
        distanceAwayFree = distanceAway;
        savedRigToGoal = RigToGoalDirection;
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
        //Pull values from controller
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");
        bool backButton = Input.GetButton("Back");

        characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);

        Vector3 lookAt = characterOffset;

        Vector3 targetPosition = Vector3.zero;

        //Determine camera state
        if (Input.GetAxis("Target") > TARGET_TRIGGER_TRESHHOLD) {
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, wideScreen, targetingTime);
            camState = CamStates.Target;
        }
        else {
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);

            //*First Person*
            if (rightY < fpsThreshold && camState != CamStates.FirstPerson &&  camState != CamStates.Free && !characterLogic.IsInLocomotion()) {
                //reset look before entering the first person mode
                xAxisRot = 0;
                lookWeight = 0f;
                camState = CamStates.FirstPerson;
            }

            //*Free Camera*
            if (rightY > freeThreshold && System.Math.Round(characterLogic.Speed, 2) == 0) {
                camState = CamStates.Free;
                savedRigToGoal = Vector3.zero;
            }

            //*Behind the back*
            if ((camState == CamStates.FirstPerson && Input.GetButton("B")) ||
                camState == CamStates.Target && (Input.GetAxis("Target") <= TARGET_TRIGGER_TRESHHOLD)) {
                camState = CamStates.Behind;
            }
         }

        switch (camState) {
            case CamStates.Behind:
                ResetCamera();

                //Only update camera look direction if moving
                if (characterLogic.Speed > characterLogic.LocomotionThreshold && characterLogic.IsInLocomotion() && !characterLogic.IsInPivot()) {
                    //the direction we wanna look
                    lookDir = Vector3.Lerp(followTransform.right * (leftX < 0 ? 1f : -1f),
                        followTransform.forward * (leftY < 0 ? -1f : 1f),
                        Mathf.Abs(Vector3.Dot(this.transform.forward, followTransform.forward)));

                    //calculate direction from camera to player, kill Y, normalize
                    currLookDir = Vector3.Normalize(characterOffset - this.transform.position);
                    currLookDir.y = 0;

                    //Damping makes it so we dont update targetPos while pivoting; camera shouldnt rotate around player
                    currLookDir = Vector3.SmoothDamp(currLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
                }

                targetPosition = characterOffset + followTransform.up * distanceUp - Vector3.Normalize(currLookDir) * distanceAway;

                break;
            case CamStates.Target:
                ResetCamera();

                lookDir = followTransform.forward;
                currLookDir = followTransform.forward;

                //setting the target position to be the correct offset from the target
                targetPosition = characterOffset + followTransform.up * distanceUp - lookDir * distanceAway;

                //zero out the lookweight
                lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);

                break;
            case CamStates.FirstPerson:
                //Looking up and down
                //calculare the amount of rotation and apply to the fspCamPos
                xAxisRot += (leftY * firstPersonLookSpeed);
                xAxisRot = Mathf.Clamp(xAxisRot, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
                firstPersonCamPos.Transform.localRotation = Quaternion.Euler(xAxisRot, 0, 0);

                //Superimpose fpsCamPos GOs rotation on camera
                Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCamPos.Transform.forward);
                this.transform.rotation = rotationShift * this.transform.rotation;

                //move characters models head up and down
                lookWeight = Mathf.Lerp(lookWeight, 1.0f, Time.deltaTime * firstPersonLookSpeed);
                headLookAt = firstPersonCamPos.Transform.position + firstPersonCamPos.Transform.forward;

                //Looking left and right
                //similar to how char is rotated while in locomotion
                Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, fPSRotationDegreePerSecond * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
                Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
                characterLogic.transform.rotation = characterLogic.transform.rotation * deltaRotation;

                //Move camera to firstPersonCamPos
                targetPosition = firstPersonCamPos.Transform.position;

                //smoothly transitionlook direction towards fpsCamPos when entering first person mode
                lookAt = Vector3.Lerp(targetPosition + followTransform.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);

                //choose lookAt target based on distance
                lookAt = Vector3.Lerp(this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonCamPos.Transform.position));
                
                break;
            case CamStates.Free:
                lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);

                Vector3 rigToGoal = characterOffset - transform.position;
				rigToGoal.y = 0f;
                Debug.DrawRay(transform.transform.position, rigToGoal, Color.red);
				
				// Panning in and out
				// If statement works for positive values; don't tween if stick not increasing in either direction; also don't tween if user is rotating
				// Checked against rightStickThreshold because very small values for rightY mess up the Lerp function
				if (rightY < lastStickMin && rightY < -1f * rightStickThreshold && rightY <= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
				{
					// Zooming out
					distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs(rightY));
					distanceAwayFree = Mathf.Lerp(distanceAway, distanceAway * distanceAwayMultiplier, Mathf.Abs(rightY));
					targetPosition = characterOffset + followTransform.up * distanceUpFree - RigToGoalDirection * distanceAwayFree;
					lastStickMin = rightY;
                }
				else if (rightY > rightStickThreshold && rightY >= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
				{
                	// Zooming in
                	// Subtract height of camera from height of player to find Y distance
					distanceUpFree = Mathf.Lerp(Mathf.Abs(transform.position.y - characterOffset.y), camMinDistFromChar.y, rightY);
					// Use magnitude function to find X distance	
					distanceAwayFree = Mathf.Lerp(rigToGoal.magnitude, camMinDistFromChar.x, rightY);		
					targetPosition = characterOffset + followTransform.up * distanceUpFree - RigToGoalDirection * distanceAwayFree;		
					lastStickMin = float.PositiveInfinity;
				}				
                                
				// Store direction only if right stick inactive
                if (Mathf.Abs(rightX) < rightStickThreshold || Mathf.Abs(rightY) < rightStickThreshold)
				{
					savedRigToGoal = RigToGoalDirection;
				}
				
			
				// Rotating around character
				parentRig.RotateAround(characterOffset,
                    followTransform.up, 
                    freeRotationDegreePerSecond * (Mathf.Abs(rightX) > rightStickThreshold ? rightX : 0f));
							
				// Still need to track camera behind player even if they aren't using the right stick; achieve this by saving distanceAwayFree every frame
				if (targetPosition == Vector3.zero)
				{
					targetPosition = characterOffset + followTransform.up * distanceUpFree - savedRigToGoal * distanceAwayFree;
				}

                break;

			case CamStates.Map:
				//Move camera to firstPersonCamPos
				targetPosition = firstPersonCamPos.Transform.position;
				break;
        }

        //set the lookat weight - amount to use look at IK vs using
        //the heads animation
        characterLogic.setLookVars(headLookAt, lookWeight);

        //compensate for walls, wall collision
        CompensateForWalls(characterOffset, ref targetPosition);

        //making smooth transition between it's current pos and the pos it want to be in
        SmoothPosition(this.transform.position, targetPosition);

        //make sure the cam is looking the rightway
        transform.LookAt(lookAt);

        rightStickPrevFrame = new Vector2(rightX, rightY);
    }

    #endregion

    #region Methods

    private void ResetCamera()
    {
        lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
    }

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