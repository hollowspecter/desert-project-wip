using UnityEngine;
using System.Collections;

public class ThreadTest : MonoBehaviour
{
    MeshJob myJob;
    void Start()
    {
        Debug.Log("Starting the Job");
        myJob = new MeshJob();
        myJob.InData = new Vector3[10];
        myJob.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.
    }
    void Update()
    {
        if (myJob != null) {
            if (myJob.Update()) {
                // Alternative to the OnFinished callback
                myJob = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (myJob != null)
            myJob.Abort();
    }
}
