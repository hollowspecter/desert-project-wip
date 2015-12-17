using UnityEngine;
using System.Collections;

public class ToolMenu : MonoBehaviour
{ 
    [SerializeField]
    private Transform _arrowT;
    [SerializeField]
    private Transform _menuUI;

    private bool activated = false;
    private float angle;
    // Use this for initialization
    void Start ()
    {
        Deactivate();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(activated)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector2 axisVector = new Vector2(h, v);
            
            if (axisVector != Vector2.zero)
            {
                angle = Vector2.Angle(Vector2.up, axisVector);
                angle *= (axisVector.x > 0f) ? -1f : 1f;
            }
            // Activate this code if you want the arrow to "snap back" when no input
            //else
            //    angle = 0;

            // Select an item

            Vector3 eulerAngles = new Vector3(0, 0, angle);
            _arrowT.localRotation = Quaternion.Euler(eulerAngles);
        }
	}

    public void Activate()
    {
        activated = true;
        _menuUI.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        activated = false;
        _menuUI.gameObject.SetActive(false);
    }
}
