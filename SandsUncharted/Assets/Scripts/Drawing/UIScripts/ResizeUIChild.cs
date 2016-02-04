using UnityEngine;
using System.Collections;

public class ResizeUIChild : MonoBehaviour
{
    [SerializeField]
    RectTransform parentRectT;
    RectTransform ownRectT;

    float standardSize = 500f;

	// Use this for initialization
	void Start ()
    {
        ownRectT = GetComponent<RectTransform>();

        float parentHeight = parentRectT.sizeDelta.y;
        float factor = parentHeight/ standardSize;
        Debug.Log("sizefactor" + factor);
        float ownSize = ownRectT.sizeDelta.x;
        ownRectT.sizeDelta = new Vector2(ownSize * factor, ownSize * factor);
        Vector2 pos = ownRectT.anchoredPosition;
        ownRectT.anchoredPosition = pos * factor;
	}
}
