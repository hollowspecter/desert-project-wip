using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string interactionString = "Interact"; //The string to be displayed next to the button prompt

    private SphereCollider coll; //The sphere trigger of this object

    private InteractionManager manager; //The Interaction manager on the player

    private Vector3 LookAtPosition; //The position the Camera should look at when the Object is interacted upon

    private float maxInteractionAngle = 120f; //The maximum angle at which the Object can be interacted with, centered around the forward axis of the object

    void Awake()
    {
        Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}


    //Initialization method
    void Init()
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
    }

    //This method will hold the objects specific interaction
    public void Interact()
    {
        Debug.Log("I have been interacted with");
    }

    void OnTriggerEnter(Collider c)
    {
        if(c.CompareTag("Player") && CheckInteractionAngle())
        {
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

    //Check if the Angle to the player is within the Interaction angle
    bool CheckInteractionAngle()
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
        //Gizmos.color = CheckInteractionAngle() ? Color.green : Color.red;
        //if(manager != null)
            //Gizmos.DrawLine(manager.transform.position, transform.position);
        
    }
}
