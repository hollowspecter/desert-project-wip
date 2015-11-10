using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class NotebookDebugger : MonoBehaviour {

    [SerializeField]
    private GameObject debugUI;

    private GameObject notebookPanel;
    private bool notebookOn = false;


    void Start()
    {
        notebookPanel = debugUI.transform.FindChild("Notebook").gameObject;
        Assert.IsNotNull<GameObject>(notebookPanel);
    }

    bool ToggleNotebook()
    {
        notebookOn = !notebookOn;
        notebookPanel.SetActive(notebookOn);
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
