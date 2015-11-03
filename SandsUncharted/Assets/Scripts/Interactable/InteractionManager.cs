using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{

    private List<Interactable> interactables; //List containing all the Interactables that we can currently interact with

    private Interactable currInteractable; //The currently selected Interactable

    private Camera _cam; // a reference to the main camera

	// Use this for initialization
	void Start ()
    {
        interactables = new List<Interactable>();
        //_cam = GameObject.Find("Camera").transform.FindChild("MainCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        ChooseCurrInteractable();
	}

    public void AddInteractable(Interactable i)
    {
        interactables.Add(i);
    }

    public void RemoveInteractable(Interactable i)
    {
        interactables.Remove(i);
    }

    //Change the interactiontarget to the next or last interactable on the list
    public void SwitchCurrInteractable(bool next)
    {
        int index = interactables.IndexOf(currInteractable);

        //if we want the next in the list, try to get index + 1. if we are at the end, jump to the first index
        if(next)
        {
            index = (index + 1) % interactables.Count;
        }
        //if we want the last in the list, try to get index - 1. if we are at the beginning, jump to the last index
        else
        {
            index = index - 1 < 0 ? interactables.Count - 1 : index - 1;
        }
    }
    //Automatically choose the "best" currInteractable based on viewing direction and distance
    void ChooseCurrInteractable()
    {
        float candidateDistance = float.MaxValue;
        float candidateAngle, maxAngle = 60f;
        Interactable bestCandidate = null;

        foreach(Interactable i in interactables)
        {
            float distance = Vector3.Distance(i.transform.position, transform.position);
            float angle = Vector3.Angle(Vector3.ProjectOnPlane((i.transform.position - transform.position), Vector3.up), Vector3.ProjectOnPlane(transform.forward, Vector3.up));
            
            //if the angle between the cameradirection and the vector to the object is low enough and the distance is closer than the distance to the current candidate
            if (angle < maxAngle && candidateDistance > distance)
            {
                candidateDistance = distance;
                candidateAngle = angle;
                bestCandidate = i;
                Debug.Log("foundya");
            }
        }
        currInteractable = bestCandidate;

    }

    void OnDrawGizmos()
    {
        if(currInteractable != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currInteractable.transform.position);
        }
    }
}
