using UnityEngine;
using System.Collections;
using System;

public class ReadInteractable : Interactable
{

    protected Vector3 LookAtPosition; //The position the Camera should look at when the Object is interacted upon

    public override void Init()
    {
        interactionString = "Read";
    }

    public override void Interact()
    {
        Debug.Log("Move the camera to the beat!");
    }
}
