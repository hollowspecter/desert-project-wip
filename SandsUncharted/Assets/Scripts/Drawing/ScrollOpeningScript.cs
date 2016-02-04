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
    private bool done = true;

    private float mapWidth;
    private float mapHeight;
    

    // Use this for initialization
    void Start ()
    {
        float mapWHratio = 16 / 9.0f;
        ownRectT = GetComponent<RectTransform>();
        mapWidth = Screen.width;
        mapWidth -= mapWidth / 20;
        mapWidth = Mathf.Min(1024 * 1.25f, mapWidth);
        mapHeight = (int)(mapWidth / mapWHratio);

        ownRectT.sizeDelta = new Vector2(0, mapHeight);
        RectTransform t = ownRectT.Find("RightBorder").GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(t.sizeDelta.x, mapHeight * 1.0f);
        t = ownRectT.Find("LeftBorder").GetComponent<RectTransform>();
        t.sizeDelta = new Vector2(t.sizeDelta.x, mapHeight * 1.0f);
    }

    // Update is called once per frame
    void Update ()
    {
        currentLerptime += Time.deltaTime;
        if (currentLerptime > DURATION)
        {
            done = true;
            currentLerptime = DURATION;
        }

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

        if(Input.GetKeyDown(KeyCode.U) && done)
        {
            ResetTimer();
        }
    }
    void ResetTimer()
    {
        opened = !opened;
        done = false;
        if (currentLerptime >= DURATION)
            currentLerptime = 0f;
        else
            currentLerptime = DURATION - currentLerptime;
    }
}
