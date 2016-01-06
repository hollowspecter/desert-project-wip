using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderTexDrawing : MonoBehaviour
{
    [SerializeField]
    private Transform _cursor;
    [SerializeField]
    private Camera _renderCam;
    [SerializeField]
    private GameObject _splineRenderTarget;

    [SerializeField]
    private GameObject captureTest;
    [SerializeField]
    private GameObject captureTest2;

    #region splinedrawing members
    List<CatmullRomSpline> splines;
    CatmullRomSpline activeSpline;
	ControlPointGroup ctrl;
    Vector3 speed;
    float acceleration = 25f;
    float selectionDistance = 0.5f;

    private int captureResolution = 1024;

    #endregion

    #region stamping members
    StampManager _stampManager;
    float rotationSpeed = 40f;
    float scaleSpeed = 5f;
    #endregion

    private ToolMenu _toolMenu;
    [SerializeField]
    private MeshLine _line;

    // Use this for initialization
    void Start()
    {
        splines = new List<CatmullRomSpline>();
        activeSpline = new CatmullRomSpline(_splineRenderTarget, GetComponent<ControlPointRenderer>());
        splines.Add(activeSpline);
        ctrl = activeSpline.ControlPoints;

        _toolMenu = GetComponent<ToolMenu>();
        _stampManager = GetComponent<StampManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //pressing Y opens the Toolmenu, this blocks all other input
        if (Input.GetButtonDown("Y"))
        {
            _toolMenu.Activate();
            /*
            if(activeSpline != null)
            {
                Debug.Log("test");
                CaptureRenderTex();
                activeSpline = null;
                ctrl = null;
            }
            _cursor.GetComponentInChildren<SpriteRenderer>().sprite = _stampManager.GetSelected();

            _stampManager.StampSelectedImage(offsetCursor, _cursor.localRotation, _cursor.localScale.x * 0.3f);
            */
        }

        if (Input.GetButtonUp("Y"))
        {
            _toolMenu.Deactivate();
        }

        //If the Toolmenu is not open, process other inputs
        if(!Input.GetButton("Y"))
        { 
            //The Left-Stick movement is used for CursorMovement
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            UpdateCursorPosition(h, v);

            Vector3 offsetCursor = new Vector3(_cursor.position.x, _cursor.position.y, _cursor.position.z + 0.1f);

            //The Right-Stick movement is used for CursorRotation(xAxis) and -Scaling(yAxis) MOVETO:StampTool
            float rX = Input.GetAxis("RightStickX");
            float rY = Input.GetAxis("RightStickY");
            if (Mathf.Abs(rX) > 0.5f && Mathf.Abs(rY) < 0.5f)
            {
                _cursor.Rotate(new Vector3(0f, 0f, Mathf.Sign(rX) * rotationSpeed * Time.deltaTime));
            }
            else if (Mathf.Abs(rX) < 0.5f && Mathf.Abs(rY) > 0.5f)
            {
                _cursor.localScale += Mathf.Sign(rY) * new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;
                float clamped = Mathf.Clamp(_cursor.localScale.x, 0.5f, 4f);
                _cursor.localScale = new Vector3(clamped, clamped, clamped);
            }

            //if there is a Spline selected MOVETO:SplineTool
            if (activeSpline != null)
            {
                activeSpline.Update();

                //check if the cursor is above it to measure the distance
                Vector3 closestPoint = ctrl.GetClosestPoint(offsetCursor);
                _line.ClearPoints();
                if (ctrl.Count >= 2)
                {
                    int insertIndex = ctrl.FindInsertIndex(offsetCursor);
                    if (insertIndex != ctrl.Count)
                    {
                        _line.SetStart(ctrl[insertIndex - 1]);
                        _line.SetEnd(ctrl[insertIndex]);
                    }
                    else if(ctrl.SelectedIndex < 0 || ! (ctrl[ctrl.SelectedIndex] == _line.GetStart()) || !(ctrl[ctrl.SelectedIndex] == _line.GetEnd()))
                    {
                        _line.ClearMesh();
                    }
                }

                //pressing A selects or moves a point if close to one
                if (Input.GetButton("A") && (ctrl.SelectedIndex >= 0) && Vector3.Distance(offsetCursor, ctrl[ctrl.SelectedIndex]) < selectionDistance)
                {
                    ctrl.MoveControlPoint(ctrl.SelectedIndex, speed * Time.deltaTime);
                }
                //and makes a new one otherwise
                else if (Input.GetButtonDown("A"))
                {
                    if (Vector3.Distance(closestPoint, offsetCursor) < selectionDistance)
                    {
                        if (ctrl.SelectedIndex != ctrl.IndexOf(closestPoint))
                            ctrl.SelectedIndex = ctrl.IndexOf(closestPoint);
                        else
                            ctrl.SelectedIndex = -1;
                    }
                    else
                    {
                        Vector3 pos = offsetCursor;
                        int index = ctrl.FindInsertIndex(pos);
                        ctrl.Insert(pos, index);
                    }
                }
                //pressing B deletes a point if close to one
                else if(Input.GetButtonDown("B"))
                {
                    if (Vector3.Distance(closestPoint, offsetCursor) < selectionDistance)
                    {
                        ctrl.Remove(closestPoint);
                    }
                }
                //pressing x finishes the spline
                else if(Input.GetButtonDown("X"))
                {
                    CaptureRenderTex();
                    activeSpline = null;
                    ctrl = null;
                }

            }
            //if there is no spline selected MOVETO:SplineTool
            else
            {
                //pressing A selects a spline if close to one
                if(Input.GetButtonDown("A"))
                {
                    _cursor.rotation = Quaternion.identity;
                    _cursor.localScale = new Vector3(1f, 1f, 1f);
                    for(int i = 0; i < splines.Count - 1; ++i)
                    {
                        if(splines[i].ControlPoints.IsCloseToSpline(offsetCursor))
                        {
                            activeSpline = splines[i];
                            ctrl = activeSpline.ControlPoints;
                            break;
                        }
                    }
                    //creates a new one otherwise
                    if(activeSpline == null)
                    {
                        activeSpline = new CatmullRomSpline(_splineRenderTarget, GetComponent<ControlPointRenderer>());
                        splines.Add(activeSpline);
                        ctrl = activeSpline.ControlPoints;
                    }
                }
            
            }

        }

        speed *= 0.75f;//friction
    }

    void OnDrawGizmos()
    {

    }


    //Saves the current RenderTexture to the backgroundTexture by snapshotting a temporary RenderTexture with the CaptureCamera
    void CaptureRenderTex()
    {
        //initialize camera and texture
        Camera captureCamera = transform.Find("CaptureCamera").GetComponent<Camera>();
        captureCamera.enabled = true;
        RenderTexture rt = new RenderTexture(captureResolution, captureResolution, 24);

        //activate rendertexture and link camera
        captureCamera.targetTexture = rt;
        captureCamera.Render();
        RenderTexture original = RenderTexture.active;
        RenderTexture.active = rt;      

        //snapshot
        Texture2D t = new Texture2D(captureResolution, captureResolution, TextureFormat.ARGB32, true);
        t.ReadPixels(new Rect(0, 0, captureResolution, captureResolution), 0, 0);
        t.Apply(true);
        captureTest.GetComponent<MeshRenderer>().materials[0].mainTexture = t;
        captureTest2.GetComponent<MeshRenderer>().materials[0].mainTexture = t;
        
        //remove the temporary stuff
        captureCamera.targetTexture = null;
        captureCamera.enabled = false;
        RenderTexture.active = original;
        DestroyImmediate(rt);
        _stampManager.RemoveAll();
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
}
