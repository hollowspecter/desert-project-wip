using UnityEngine;
using System.Collections;

public class RenderTexDrawing : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Transform positionParent;
    [SerializeField]
    private Transform cursor;
    [SerializeField]
    private Camera _renderCam;
    [SerializeField]
    private Texture crossimage;

    CatmullRomSpline spline;

    Vector3 speed;
    // Use this for initialization
    void Start ()
    {
        spline = GetComponent<CatmullRomSpline>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 axisVector = h * transform.right + v * transform.up;
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();

        /****move Reticle based on acceleration and right stick vector****/
        speed += 50f * axisVector * Time.deltaTime; //make velocityvector
        cursor.transform.position += speed * Time.deltaTime; //make movementvector
        //brushCircle.transform.rotation = transform.parent.rotation; //negate the maps rotation to keep the pencil straight (not functional as the brush doesnt actually rotate)
        speed *= 0.75f;//friction

        if (Input.GetButtonDown("A"))
        {
            spline.AddControlPoint(new Vector3(cursor.position.x, cursor.position.y, positionParent.position.z));
        }

        else if (Input.GetButtonDown("B"))
        {
            spline.RemoveControlPointAt(spline.GetControlPointCount() - 1);
        }

        DrawPoints();
	}

    void DrawPoints()
    {
        int count = spline.GetControlPointCount();
        if (count > 0)
        {
            for (int i = 0; i < count; ++i)
            {
                
            }
        }
    }
}
