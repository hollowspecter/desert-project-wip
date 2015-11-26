using UnityEngine;
using System.Collections;

public class RenderTexDrawing : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Transform cursor;
    [SerializeField]
    private Camera _renderCam;

    CatmullRomSpline spline;
	ControlPointGroup ctrl;
    Vector3 speed;

	float selectionDistance = 0.5f;

    // Use this for initialization
    void Start ()
    {
        spline = GetComponent<CatmullRomSpline>();
		ctrl = spline.ControlPoints;
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
        cursor.position += speed * Time.deltaTime; //make movementvector
        //brushCircle.transform.rotation = transform.parent.rotation; //negate the maps rotation to keep the pencil straight (not functional as the brush doesnt actually rotate)
        
        Vector3 closestPoint = ctrl.GetClosestPoint(cursor.position);

		if (Input.GetButton ("A") && (ctrl.SelectedIndex >= 0) && Vector3.Distance(cursor.position, ctrl[ctrl.SelectedIndex]) < selectionDistance ) 
		{
			ctrl.MoveControlPoint(ctrl.SelectedIndex, speed *Time.deltaTime);
		}

        else if (Input.GetButtonDown("A"))
        {	
			if(Vector3.Distance(closestPoint, cursor.position) < selectionDistance)
			{
				if(ctrl.SelectedIndex != ctrl.IndexOf(closestPoint))
					ctrl.SelectedIndex = ctrl.IndexOf(closestPoint);
				else
					ctrl.SelectedIndex = -1;
			}
			else
			{
                Vector3 pos = new Vector3(cursor.position.x, cursor.position.y, cursor.position.z + 0.1f);
                int index = ctrl.FindInsertIndex(pos);
                ctrl.Insert(pos, index);
			}
        }

        else if (Input.GetButtonDown("B"))
        {
            if (Vector3.Distance(closestPoint, cursor.position) < selectionDistance)
            {
                ctrl.Remove(closestPoint);
            }
        }

		speed *= 0.75f;//friction
	}
}
