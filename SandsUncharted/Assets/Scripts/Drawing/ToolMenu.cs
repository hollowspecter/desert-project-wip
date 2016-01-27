﻿using UnityEngine;
using System.Collections;

public class ToolMenu : MonoBehaviour
{ 
    [SerializeField]
    private Transform _arrowT;
    [SerializeField]
    private Transform _menuUI;
    private RectTransform[] items;

    private bool activated = false;
    private float angle;

    int toolIndex;
    int numOfTools = 4;

    // Use this for initialization
    void Start ()
    {
        items = new RectTransform[4];
        Transform itemParent = _menuUI.Find("items");
        for (int i = 0; i < numOfTools; ++i)
        {
            items[i] = (RectTransform) itemParent.GetChild(i);
        }

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
            toolIndex  = GetSelected(numOfTools, angle);

            Vector3 eulerAngles = new Vector3(0, 0, angle);
            _arrowT.localRotation = Quaternion.Euler(eulerAngles);
        }

        //rescale All Items so that the selected one is bigger
        for(int i = 0; i < numOfTools; ++i)
        {
            if(i == toolIndex)
            {
                items[i].sizeDelta = new Vector2(100, 100); 
            }
            else
            {
                items[i].sizeDelta = new Vector2(70, 70); 
            }
        }
	}
    int GetSelected(int numberOfItems, float angle)
    {
        // First, map the angles on 0 to 360 degree
        float tmpAngle = this.angle;
        if (tmpAngle < 0) tmpAngle *= -1f;
        else if (tmpAngle > 0)
        {
            float newAngle = 180;
            newAngle += 180f - (tmpAngle % 360f);
            tmpAngle = newAngle;
        }

        float step = 360f / ((float)numberOfItems);
        tmpAngle = (tmpAngle /*- (step / 2f)*/) % 360f;
        return Mathf.RoundToInt(tmpAngle / step) % numberOfItems;
    }

    public void Activate()
    {
        activated = true;
        _menuUI.gameObject.SetActive(true);
    }

    public int Deactivate()
    {
        activated = false;
        _menuUI.gameObject.SetActive(false);
        return toolIndex;
    }
}