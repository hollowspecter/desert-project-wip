using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField]
    private ButtonPrompt buttonPrompt;
    
    private List<Interactable> interactables; //List containing all the Interactables that we can currently interact with
    private Interactable currInteractable; //The currently selected Interactable
    private Interactable interactingWith; //the interactable you are or have interacted with
    private Transform _camT; // a reference to the main camera
    private bool canInteract = true;
    private Transform _transform;
    

	// Use this for initialization
	void Start ()
    {
        _transform = GetComponent<Transform>();
        interactables = new List<Interactable>();
        _camT = GameObject.Find("Camera").transform.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update ()
    {
        ChooseCurrInteractable();

        if(currInteractable != null && currInteractable.CheckInteractionAngle() && canInteract)
        {
            buttonPrompt.Activate(currInteractable.GetInteractionString());
        }
        else
        {
            buttonPrompt.Deactivate();
        }
	}

    public void AddInteractable(Interactable i)
    {
        interactables.Add(i);
    }

    public void RemoveInteractable(Interactable i)
    {
        if (interactables.Contains(i))
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
            if (i.CheckInteractionAngle())
            {
                float distance = Vector3.Distance(i.transform.position, transform.position);
                float angle = Vector3.Angle(Vector3.ProjectOnPlane((i.transform.position - _camT.position), Vector3.up), Vector3.ProjectOnPlane(_camT.forward, Vector3.up));

                //if the angle between the cameradirection and the vector to the object is low enough and the distance is closer than the distance to the current candidate
                if (angle < maxAngle && candidateDistance > distance)
                {
                    candidateDistance = distance;
                    candidateAngle = angle;
                    bestCandidate = i;
                }
            }
        }
        currInteractable = bestCandidate;

    }
    
    public void SetCanInteract(bool b)
    {
        canInteract = b;
    }
    
    public void CanInteract(){canInteract = true;}
    public void CannotInteract(){canInteract = false;}

    public bool GetCanInteract()
    {
        return canInteract;
    }

    public Interactable GetInteractable()
    {
        return interactingWith;
    }

    void OnDrawGizmos()
    {
        if(currInteractable != null)
        {
            Gizmos.color = currInteractable.CheckInteractionAngle() ? Color.blue : Color.red;
            Gizmos.DrawLine(transform.position, currInteractable.transform.position);
        }
    }

    #region State delegate Input Handling

    bool Interact()
    {
        if (currInteractable != null) {
            TurnTo();
            currInteractable.Interact();
            interactingWith = currInteractable;
            return true;
        }
        else
            return false;
    }

    void TurnTo()
    {
        Vector3 interactablePosition = currInteractable.transform.position;
        interactablePosition = interactablePosition - _transform.position;
        interactablePosition.y = 0;
        float angle = Vector3.Angle(_transform.forward, interactablePosition);
        float sign = Vector3.Cross(_transform.forward, interactablePosition).y;
        sign = (sign < 0) ? -1 : 1;
        _transform.Rotate(_transform.up, angle * sign);
    }

    void OnEnable()
    {
        TargetState.Interact += Interact;
        BehindBackState.Interact += Interact;
        InteractionState.OnEnter += CannotInteract;
        InteractionState.OnExit += CanInteract;
    }

    void OnDisable()
    {
        TargetState.Interact -= Interact;
        BehindBackState.Interact -= Interact;
        InteractionState.OnEnter -= CannotInteract;
        InteractionState.OnExit -= CanInteract;
    }

    #endregion
}
