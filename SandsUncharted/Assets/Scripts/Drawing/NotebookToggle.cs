using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class NotebookToggle : MonoBehaviour
{
    private GameObject notebook;

    private bool notebookOn = false;


    void Start()
    {
        notebook = transform.Find("Notebook").gameObject;
        Assert.IsNotNull<GameObject>(notebook);
    }

    bool ToggleNotebook()
    {
        notebookOn = !notebookOn; //switch
        notebook.SetActive(notebookOn); //update status
        return notebookOn;
    }

    bool CanSwitchToNotebook()
    {
        return notebookOn;
    }

    void CloseNotebook()
    {
        if (notebookOn)
            ToggleNotebook();
    }

    void OnEnable()
    {
        MapState.ToggleNotebook += ToggleNotebook;
        BehindBackState.ToggleNotebook += ToggleNotebook;
        NotebookState.PutNotebookBack += ToggleNotebook;
        FirstPersonState.ToggleNotebook += ToggleNotebook;
        InteractionState.ToggleNotebook += ToggleNotebook;

        MapState.SwitchToNotebook += CanSwitchToNotebook;
        FirstPersonState.SwitchToNotebook += CanSwitchToNotebook;
        InteractionState.SwitchToNotebook += CanSwitchToNotebook;

        InteractionState.CloseNotebook += CloseNotebook;
    }

    void OnDisable()
    {
        MapState.ToggleNotebook -= ToggleNotebook;
        BehindBackState.ToggleNotebook -= ToggleNotebook;
        NotebookState.PutNotebookBack -= ToggleNotebook;
        FirstPersonState.ToggleNotebook -= ToggleNotebook;
        InteractionState.ToggleNotebook -= ToggleNotebook;

        MapState.SwitchToNotebook -= CanSwitchToNotebook;
        FirstPersonState.SwitchToNotebook -= CanSwitchToNotebook;
        InteractionState.SwitchToNotebook -= CanSwitchToNotebook;

        InteractionState.CloseNotebook -= CloseNotebook;
    }
}
