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
        manager.SetCanInteract(!manager.GetCanInteract());
        if(dialoguePanel != null)
            dialoguePanel.SetActive(!dialoguePanel.activeSelf);

    }
}
