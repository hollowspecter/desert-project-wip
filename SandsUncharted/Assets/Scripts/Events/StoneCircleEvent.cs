using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class StoneCircleEvent : TimeEvent 
{
    [SerializeField]
    private float distanceToPlayer = 5f;
    [SerializeField]
    private float bulletTimeDuration = 20f;
    [SerializeField]
    private CameraEffects camEffects;

    private Transform player;
    private float timer = float.MaxValue;
    private bool eventFired = false;

    protected override void OverrideAwake()
    {
        Assert.IsNotNull<CameraEffects>(camEffects);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull<Transform>(player);
    }

    protected override void OverrideUpdate()
    {
        if (eventFired) {
            if (timer < bulletTimeDuration) {
                timer += Time.deltaTime;
                return;
            }
            else {
                TimeManager.Instance().TimeScale = 1f;
                camEffects.Toggle();
                eventFired = false;
            }
        }
    }

    protected override bool CheckConditionsToFire()
    {
        Debug.Log("Checking Conditions for Event");
        return ((transform.position - player.position).magnitude < distanceToPlayer) && !eventFired;
    }

    protected override void FireEvent()
    {
        Debug.Log("Event Fired!");
        timer = 0f;
        eventFired = true;
        TimeManager.Instance().TimeScale = 0.01f;
        camEffects.Toggle();
    }

    
}
