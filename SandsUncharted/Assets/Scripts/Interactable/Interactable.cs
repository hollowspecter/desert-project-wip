using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    protected bool isInteractable = true;

    protected string interactionString = "Interact"; //The string to be displayed next to the button prompt

    private SphereCollider coll; //The sphere trigger of this object

    protected InteractionManager manager; //The Interaction manager on the player

    [SerializeField]
    protected float maxInteractionAngle = 120f; //The maximum angle at which the Object can be interacted with, centered around the forward axis of the object

    void Awake()
    {
        coll = GetComponent<SphereCollider>();
        if (coll == null)
        {
            Debug.LogError("spherecollider is null");
        }
        manager = GameObject.FindGameObjectWithTag("Player").GetComponent<InteractionManager>();
        if (manager == null)
        {
            Debug.LogError("manager is null");
        }

        Init();
    }

    //Initialization method
    public abstract void Init();

    //This method will hold the objects specific interaction
    public abstract void Interact();


    void OnTriggerEnter(Collider c)
    {
        if(c.CompareTag("Player") && CheckInteractionAngle())
        {
            if (isInteractable)
                manager.AddInteractable(this);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Player") && CheckInteractionAngle())
        {
            manager.RemoveInteractable(this);
        }
    }

    public void Deactivate()
    {
        manager.RemoveInteractable(this);
        isInteractable = false;
    }

    //Check if the Angle to the player is within the Interaction angle
    public bool CheckInteractionAngle()
    {
        if (manager != null)
        {
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            Vector3 flatPlayerVector = Vector3.ProjectOnPlane((manager.transform.position - transform.position), Vector3.up);
            float angle = Vector3.Angle(flatForward, flatPlayerVector);

            if (angle <= maxInteractionAngle / 2)
            {
                return true;
            }

        }
        return false;
    }

    void OnDrawGizmos()
    {
        
    }

    public string GetInteractionString()
    {
        return interactionString;
    }
}
