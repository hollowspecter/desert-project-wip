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
        Quaternion northRotation = Quaternion.LookRotation(north, transform.up);
        needle.rotation = Quaternion.RotateTowards(needle.rotation, northRotation, 10f);
	}
}
