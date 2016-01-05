using UnityEngine;
using System.Collections;

public class ThumbView : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform leftT;
    [SerializeField]
    private Transform rightT;

    private bool leftClosed;
    private bool rightClosed;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        leftClosed = rightClosed = false;
        if (Input.GetAxis("TriggerR") > 0.5f || Input.GetKey(KeyCode.I))
        {
            Debug.Log("test");
            rightClosed = true;
        }
        if(Input.GetAxis("TriggerL") > 0.5f || Input.GetKey(KeyCode.U))
        {
            leftClosed = true;
        }

        Vector3 middlePos = leftT.position + (rightT.position - leftT.position)/2;
        _camera.transform.position = middlePos;

        if (rightClosed & !leftClosed)
        {
            //darkenRight
            _camera.transform.position = leftT.position;
        }
        if(!rightClosed & leftClosed)
        {
            //darkenLeft
            _camera.transform.position = rightT.position;
        }

        if(leftClosed && rightClosed)
        {
            //Make Black
        }
        
    }
}
