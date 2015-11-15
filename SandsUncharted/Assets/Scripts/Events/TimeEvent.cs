using UnityEngine;
using System.Collections;

/// <summary>
/// For specific events, derive from this class.
/// It will fire the event, if the self defined conditions are met,
/// at a certain time of day.
/// </summary>
public abstract class TimeEvent : MonoBehaviour
{
    [SerializeField]
    protected float timeOfEvent = 0f;

    private float epsilon = 0.0001f;
    private TimeManager timeMgr;

    void Awake()
    {
        timeMgr = TimeManager.Instance();
        OverrideAwake();
    }

    void Update()
    {
        if (timeOfEvent >= timeMgr.TimeOfDay - epsilon && timeOfEvent <= timeMgr.TimeOfDay + epsilon) {
            if (CheckConditionsToFire()) {
                FireEvent();
            }
        }

        OverrideUpdate();
    }

	protected abstract bool CheckConditionsToFire();
    protected abstract void FireEvent();
    protected abstract void OverrideAwake();
    protected abstract void OverrideUpdate();
}
