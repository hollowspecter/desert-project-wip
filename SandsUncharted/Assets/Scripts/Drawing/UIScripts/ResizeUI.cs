using UnityEngine;
using System.Collections;

public class ResizeUI : MonoBehaviour
{
    RectTransform ownRectT;

	// Use this for initialization
	void Awake()
    {
        float mapWHratio = 16 / 9.0f;
        ownRectT = GetComponent<RectTransform>();
        float height = Screen.height* 0.9f;
        Debug.Log("UISize" + height);
        ownRectT.sizeDelta = new Vector2(height, height);
    }
}
