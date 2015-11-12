using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ReadInteractable : Interactable
{
    [SerializeField]
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
        //manager.SetCanInteract(false);
        if (dialoguePanel != null) {
            dialoguePanel.SetActive(true);
            dialoguePanel.GetComponentInChildren<Text>().text = dialogue;
            InteractionState.InteractA += ReadAndClose;
        }
    }

    bool ReadAndClose()
    {
        if (dialoguePanel != null) {
            Debug.Log("Read and Close called in ReadInteractable");
            dialoguePanel.SetActive(false);
            //manager.SetCanInteract(true);
            InteractionState.InteractA -= ReadAndClose;
            return true;
        }
        else
            return false;
    }
}
