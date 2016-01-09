using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderTexDrawing : MonoBehaviour
{

    #region cursor variables
    [SerializeField]
    private Transform _cursor;

    Vector3 speed;
    float acceleration = 25f;
    Vector3 offsetCursor;

    #endregion

    #region capture variables

    [SerializeField]
    private Camera _renderCam;
    private RenderTexture captureRenderTexture;
    private Texture2D captureTexture;
    private Camera captureCamera;
    [SerializeField]
    private GameObject captureTarget;

    private int captureResolution = 2048;

    #endregion

    #region stamping variables
    StampManager _stampManager;
    [SerializeField]
    private Sprite[] _images;
    [SerializeField]
    private GameObject _stampPrefab;
    #endregion

    #region splineTool variables

    [SerializeField]
    private GameObject _splineRenderTarget;
    [SerializeField]
    private Sprite circleCursor;
    #endregion

    #region eraserTool variables
    [SerializeField]
    private Sprite eraserSprite;
    [SerializeField]
    private Sprite eraserCursor;
    #endregion

    #region meterTool
    [SerializeField]
    private Sprite meterCursor;
    #endregion
    [SerializeField]
    private MeshLine _line;
    private ToolMenu _toolMenu;

    private ITool activeTool;
    private SplineTool splineTool;
    private StampTool stampTool;
    private MeterTool meterTool;
    private EraserTool eraserTool;

    private bool captureFrame = true;

    // Use this for initialization
    void Start()
    {
        _toolMenu = GetComponent<ToolMenu>();
        _stampManager = GetComponent<StampManager>();
        captureTexture = new Texture2D(captureResolution, captureResolution, TextureFormat.ARGB32, true);
        captureCamera = transform.Find("CaptureCamera").GetComponent<Camera>();


        splineTool = new SplineTool(this, _splineRenderTarget, _line, circleCursor);
        stampTool = new StampTool(this, _images, _stampPrefab);
        meterTool = new MeterTool(this, _line, meterCursor);
        eraserTool = new EraserTool(this, eraserSprite, eraserCursor);

        activeTool = splineTool;
        activeTool.Activate();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (captureFrame)
        {
            captureFrame = false;
            Debug.Log(Time.deltaTime);
        }
        //pressing Y opens the Toolmenu, this blocks all other input
        if (Input.GetButtonDown("Y"))
        {
            _toolMenu.Activate();
            activeTool.Deactivate();
        }

        if (Input.GetButtonUp("Y"))
        {
            int toolIndex = _toolMenu.Deactivate();
            switch(toolIndex)
            {
                case 0:
                    activeTool = splineTool;
                    break;
                case 1:
                    activeTool = stampTool;
                    break;
                case 2:
                    activeTool = meterTool;
                    break;
                case 3:
                    activeTool = eraserTool;
                    break;
            }
            activeTool.Activate();
        }

        //If the Toolmenu is not open, process other inputs
        if(!Input.GetButton("Y"))
        { 
            //The Left-Stick movement is used for CursorMovement
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            UpdateCursorPosition(h, v);

            offsetCursor = new Vector3(_cursor.position.x, _cursor.position.y, _cursor.position.z + 0.1f);

            //All a,b,x button inputs are delegated to the active tool
            activeTool.Update(offsetCursor, _cursor.localRotation, _cursor.localScale.x);
            if(Input.GetButton("A"))
                activeTool.ButtonA();
            if (Input.GetButtonDown("A"))
                activeTool.ButtonADown();
            if (Input.GetButtonUp("A"))
                activeTool.ButtonAUp();
            if (Input.GetButton("B"))
                activeTool.ButtonB();
            if (Input.GetButtonDown("B"))
                activeTool.ButtonBDown();
            if (Input.GetButtonUp("B"))
                activeTool.ButtonBUp();

            if (Input.GetButton("X"))
                activeTool.ButtonX();
            if (Input.GetButtonDown("X"))
                activeTool.ButtonXDown();
            if (Input.GetButtonUp("X"))
                activeTool.ButtonXUp();




            //The Right-Stick movement is used for CursorRotation(xAxis) and -Scaling(yAxis) MOVETO:StampTool
            float rX = Input.GetAxis("RightStickX");
            float rY = Input.GetAxis("RightStickY");
            if (Mathf.Abs(rX) > 0.3f || Mathf.Abs(rY) > 0.3f)
                activeTool.RightStick(rX, rY);
            
        }

        speed *= 0.75f;//friction
    }
    
    void OnDrawGizmos()
    {

    }


    //Saves the current RenderTexture to the backgroundTexture by snapshotting a temporary RenderTexture with the CaptureCamera
    public void CaptureRenderTex()
    {
        Debug.Log("Capture");
        //initialize camera and texture
        captureCamera.enabled = true;
        RenderTexture original = RenderTexture.active;
        captureRenderTexture = RenderTexture.GetTemporary(captureResolution, captureResolution);
        RenderTexture.active = captureRenderTexture;
        //activate rendertexture and link camera

        captureCamera.targetTexture = captureRenderTexture;
        captureCamera.Render();  
        //snapshot
        
        captureTexture.ReadPixels(new Rect(0, 0, captureResolution, captureResolution), 0, 0);
        captureTexture.Apply(false);
        captureFrame = true;
        captureTarget.GetComponent<MeshRenderer>().materials[0].mainTexture = captureTexture;

        //remove the temporary stuff
        RenderTexture.ReleaseTemporary(captureRenderTexture);
        //captureCamera.targetTexture = null;
        captureCamera.enabled = false;
        RenderTexture.active = original;
    }

    //move the cursor
    void UpdateCursorPosition(float h, float v)
    {
        Vector3 axisVector = h * transform.right + v * transform.up;
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();

        /****move Reticle based on acceleration and left stick vector****/
        speed += acceleration * axisVector * Time.deltaTime; //make velocityvector
        _cursor.position += speed * Time.deltaTime; //make movementvector
    }

    //Rotate(xAxis) and Scale(yAxis) the cursor
    public void RotateAndScaleCursor(float rX, float rY, float rotationSpeed, float scaleSpeed)
    {

        if (Mathf.Abs(rX) > 0.5f && Mathf.Abs(rY) < 0.5f)
        {
            _cursor.Rotate(new Vector3(0f, 0f, Mathf.Sign(rX) * rotationSpeed * Time.deltaTime));
        }
        else if (Mathf.Abs(rX) < 0.5f && Mathf.Abs(rY) > 0.5f)
        {
            _cursor.localScale += -Mathf.Sign(rY) * new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;
            float clamped = Mathf.Clamp(_cursor.localScale.x, 0.5f, 4f);
            _cursor.localScale = new Vector3(clamped, clamped, clamped);
        }
    }

    public void SetCursorImage(Sprite s)
    {
        _cursor.GetComponentInChildren<SpriteRenderer>().sprite = s;
    }

    public void ResetCursorRotScal()
    {
        _cursor.localRotation = Quaternion.identity;
        _cursor.localScale = new Vector3(1, 1, 1);
    }

    public Vector3 GetSpeed()
    {
        return speed;
    }
}
