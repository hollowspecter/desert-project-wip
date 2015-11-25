﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPointRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private Transform parentTransform;

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

    public void ShowPoints(Vector3[] positions)
    {
        if(positions.Length <= controlObjects.Length)
        {
            for (int i = 0; i < positions.Length; ++i)
            {
				controlObjects[i].gameObject.SetActive(true);
                controlObjects[i].position = positions[i];
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
}