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
    float selectionDistance = 0.5f;

    #endregion

    #region stamping members
    StampManager _stampManager;
    float rotationSpeed = 40f;
    float scaleSpeed = 5f;
    #endregion

    // Use this for initialization
    void Start()
    {
        splines = new List<CatmullRomSpline>();
        activeSpline = new CatmullRomSpline(_splineRenderTarget, GetComponent<ControlPointRenderer>());
        splines.Add(activeSpline);
        ctrl = activeSpline.ControlPoints;

        _stampManager = GetComponent<StampManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        UpdateCursorPosition(h, v);

        Vector3 offsetCursor = new Vector3(_cursor.position.x, _cursor.position.y, _cursor.position.z + 0.1f);


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

        if (Input.GetButtonDown("Y"))
        {
            if(activeSpline != null)
            {
                Debug.Log("test");
                CaptureRenderTex();
                activeSpline = null;
                ctrl = null;
            }
            _cursor.GetComponentInChildren<SpriteRenderer>().sprite = _stampManager.GetSelected();

            _stampManager.StampSelectedImage(offsetCursor, _cursor.localRotation, _cursor.localScale.x * 0.3f);
        }

        if (activeSpline != null)
        {
            activeSpline.Update();
            Vector3 closestPoint = ctrl.GetClosestPoint(offsetCursor);

            if (Input.GetButton("A") && (ctrl.SelectedIndex >= 0) && Vector3.Distance(offsetCursor, ctrl[ctrl.SelectedIndex]) < selectionDistance)
            {
                ctrl.MoveControlPoint(ctrl.SelectedIndex, speed * Time.deltaTime);
            }

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

            else if (Input.GetButtonDown("B"))
            {
                if (Vector3.Distance(closestPoint, offsetCursor) < selectionDistance)
                {
                    ctrl.Remove(closestPoint);
                }
            }
            else if(Input.GetButtonDown("X"))
            {
                CaptureRenderTex();
                activeSpline = null;
                ctrl = null;
            }

        }
        else
        {
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

                if(activeSpline == null)
                {
                    activeSpline = new CatmullRomSpline(_splineRenderTarget, GetComponent<ControlPointRenderer>());
                    splines.Add(activeSpline);
                    ctrl = activeSpline.ControlPoints;
                }
            }
            
        }

        speed *= 0.75f;//friction
	}

    void OnDrawGizmos()
    {/*
        for (int i = 0; i < splines.Count - 1; ++i)
        {
            splines[i].DrawGizmos();
        }*/
    }

    void CaptureRenderTex()
    {
        Camera captureCamera = transform.Find("CaptureCamera").GetComponent<Camera>();
        captureCamera.enabled = true;
        RenderTexture rt = new RenderTexture(512, 512, 24);

        captureCamera.targetTexture = rt;
        captureCamera.Render();
        RenderTexture original = RenderTexture.active;
        RenderTexture.active = rt;      

        Texture2D t = new Texture2D(512, 512, TextureFormat.ARGB32, true);
        t.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        t.Apply(true);
        captureTest.GetComponent<MeshRenderer>().materials[0].mainTexture = t;
        captureTest2.GetComponent<MeshRenderer>().materials[0].mainTexture = t;

        captureCamera.targetTexture = null;
        captureCamera.enabled = false;
        RenderTexture.active = original;
        DestroyImmediate(rt);
    }

    void UpdateCursorPosition(float h, float v)
    {
        Vector3 axisVector = h * transform.right + v * transform.up;
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();

        /****move Reticle based on acceleration and left stick vector****/
        speed += 15f * axisVector * Time.deltaTime; //make velocityvector
        _cursor.position += speed * Time.deltaTime; //make movementvector
    }
}
