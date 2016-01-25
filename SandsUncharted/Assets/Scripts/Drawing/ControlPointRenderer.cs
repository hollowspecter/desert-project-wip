using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPointRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Transform parentTransform;
	[SerializeField]
	private Sprite pointImage;
	[SerializeField]
	private Sprite selectImage;

    [SerializeField]
    private int objectCount = 64;

    private Transform[] controlObjects;

	// Use this for initialization
	void Start ()
    {
        controlObjects = new Transform[objectCount];
        for(int i = 0; i < objectCount; ++i)
        {
            controlObjects[i] = GameObject.Instantiate(prefab).transform;
            controlObjects[i].SetParent(parentTransform);
        }
	}

    public void ShowPoints(Vector3[] positions, int selectedIndex)
    {
        if(positions.Length <= controlObjects.Length)
        {
            for (int i = 0; i < positions.Length; ++i)
            {
				controlObjects[i].gameObject.SetActive(true);
                controlObjects[i].position = positions[i];
                controlObjects[i].localPosition = new Vector3(controlObjects[i].localPosition.x, controlObjects[i].localPosition.y, 0f);
				if(i == selectedIndex)
					controlObjects[i].GetComponent<SpriteRenderer>().sprite = selectImage;
				else
					controlObjects[i].GetComponent<SpriteRenderer>().sprite = pointImage;
            }
			for (int i = positions.Length; i < objectCount; ++i)
			{
				controlObjects[i].gameObject.SetActive(false);
			}
        }
        else
        {
            Debug.LogError("There are more points to be shown than there are transforms available");
        }
    }

    public void HidePoints()
    {
        for (int i = 0; i < objectCount; ++i)
        {
            controlObjects[i].gameObject.SetActive(false);
        }
    }
}
