using UnityEngine;
using System.Collections;

public class ScrollOpeningScript : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    private RectTransform ownRectT;
    private const float DURATION = 1f;
    private float currentLerptime = 0f;

    private bool opened = false;

    private float mapWidth;
    private float mapHeight;

    // Use this for initialization
    void Start ()
    {
        float mapWHratio = Screen.width / Screen.height;
        ownRectT = GetComponent<RectTransform>();
        mapWidth = Screen.width;
        mapWidth -= mapWidth / 20;
        mapHeight = (int)(mapWidth / mapWHratio);

        ownRectT.sizeDelta = new Vector2(0, mapHeight);

    }

    // Update is called once per frame
    void Update ()
    {
        currentLerptime += Time.deltaTime;
        if (currentLerptime > DURATION)
            currentLerptime = DURATION;

        float t = currentLerptime / DURATION;
        t = curve.Evaluate(t);

        if (opened && ownRectT.sizeDelta.x < mapWidth)
        {
            ownRectT.sizeDelta = new Vector2( Mathf.Lerp(0, mapWidth, t), mapHeight);

        }
        if (!opened && ownRectT.sizeDelta.x > 0)
        {
            ownRectT.sizeDelta = new Vector2(Mathf.Lerp(mapWidth, 0, t), mapHeight);
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            ResetTimer();
        }
    }
    void ResetTimer()
    {
        opened = !opened;
        if (currentLerptime >= DURATION)
            currentLerptime = 0f;
        else
            currentLerptime = DURATION - currentLerptime;
    }
}
