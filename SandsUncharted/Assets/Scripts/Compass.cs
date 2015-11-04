using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour
{
    Vector3 north = new Vector3(1f, 0f, 0f);

    Vector3 heading;

    Transform needle;

	// Use this for initialization
	void Start ()
    {
        needle = transform.Find("Needle");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(gameObject.activeSelf)
        {
            Quaternion northRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(north, transform.up), transform.up);
            needle.rotation = Quaternion.RotateTowards(needle.rotation, northRotation, 2f);
        }

        Rigidbody r = new Rigidbody();
	}
}
