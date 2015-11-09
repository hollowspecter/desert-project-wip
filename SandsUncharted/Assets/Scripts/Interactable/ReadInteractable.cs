using UnityEngine;
using System.Collections;
using System;

public class ReadInteractable : Interactable
{

    protected Vector3 LookAtPosition; //The position the Camera should look at when the Object is interacted upon

    private string dialogue = "";

    [SerializeField]
    protected GameObject dialoguePanel;

    public override void Init()
    {
        interactionString = "Read";
    }

    public override void Interact()
    {
        Debug.Log("Move the camera to the beat!");
        manager.SetCanInteract(false);
        if(dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    bool ReadAndClose()
    {
        if (dialoguePanel != null) {
            dialoguePanel.SetActive(false);
            manager.SetCanInteract(true);
            return true;
        }
        else
            return false;
    }

    #region State Input Handling

    void OnEnable()
    {
        InteractionState.InteractAndExit += ReadAndClose;
    }

    void OnDisable()
    {
        InteractionState.InteractAndExit -= ReadAndClose;
    }

    #endregion
}
