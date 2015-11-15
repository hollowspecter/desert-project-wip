///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    #region singleton
    // singleton
    private static TimeManager timeManager;
    public static TimeManager Instance()
    {
        if (!timeManager) {
            timeManager = FindObjectOfType(typeof(TimeManager)) as TimeManager;
            if (!timeManager)
                Debug.LogError("There needs to be one active GameManager on a GameObject in your scene.");
        }

        return timeManager;
    }
    #endregion

    #region private variables
    [SerializeField]
    private GameObject sun;
    [SerializeField]
    private Text timeDebug;
    [SerializeField]
    private float timeScale = 1f;
    [SerializeField]
    private float daytimeRLSeconds = 10.0f * 60;
    [SerializeField]
    private float duskRLSeconds = 1.5f * 60;
    [SerializeField]
    private float nighttimeRLSeconds = 7.0f * 60;
    [SerializeField]
    private float sunsetRLSeconds = 1.5f * 60;
    [SerializeField]
    private float startOfDaytime = 0f;

    private float timeRT = 0f;
    private float startOfDusk;
    private float startOfNighttime;
    private float gameDayRLSeconds;
    private float startOfSunset;

    private AutoIntensity autoIntensity;
    private Transform sunTransform;
    #endregion

    #region properties (public)
    public float TimeScale { get { return timeScale; } set { timeScale = value; } }

    public float TimeOfDay // game time 0 .. 1
    {
        get { return timeRT / gameDayRLSeconds; }
        set { timeRT = value * gameDayRLSeconds; }
    }

    public float OneHour
    {
        get { return (gameDayRLSeconds / 24f) / gameDayRLSeconds; }
    }
    #endregion
    
    #region Unity events

    void Awake()
    {
        autoIntensity = sun.GetComponent<AutoIntensity>();
        Assert.IsNotNull<AutoIntensity>(autoIntensity);

        sunTransform = sun.transform;
        Assert.IsNotNull<Transform>(sunTransform);

        gameDayRLSeconds = daytimeRLSeconds + duskRLSeconds + nighttimeRLSeconds + sunsetRLSeconds;
        startOfDusk = daytimeRLSeconds / gameDayRLSeconds;
        startOfNighttime = startOfDusk + duskRLSeconds / gameDayRLSeconds;
        startOfSunset = startOfNighttime + nighttimeRLSeconds / gameDayRLSeconds;

        TimeOfDay = startOfDaytime;
        Debug.Log("Start: " + startOfDaytime + " and Time:" + TimeOfDay);
    }

    // Update is called once per frame
    void Update()
    {
        timeRT = (timeRT + Time.deltaTime * timeScale) % gameDayRLSeconds;
        float sunangle = TimeOfDay * 360;
        sun.transform.position = Quaternion.Euler(0, 0, sunangle) * (10f * Vector3.right);
        sun.transform.LookAt(Vector3.zero);

        if (timeDebug != null) {
            timeDebug.text = TimeOfDay.ToString();
        }
    }

    #endregion

    #region methods
    void OnEnable()
    {
        BehindBackState.SkipHour += SkipHour;
        TargetState.SkipHour += SkipHour;
    }

    void OnDisable()
    {
        BehindBackState.SkipHour -= SkipHour;
        TargetState.SkipHour -= SkipHour;
    }

    void SkipHour()
    {
        TimeOfDay = TimeOfDay + OneHour;
        Debug.Log("Skipped an hour. One hour is " + OneHour.ToString());
    }
    #endregion
}
