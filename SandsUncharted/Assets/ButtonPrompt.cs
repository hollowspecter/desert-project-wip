using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonPrompt : MonoBehaviour
{
    private Animator _animator;
    private Text promptText;
    private GameObject promptPanel;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        promptPanel = transform.GetChild(0).gameObject;
        promptText = GetComponentInChildren<Text>();
        Deactivate();
    }

    public void Activate(string promptString)
    {
        promptText.text = promptString;
        _animator.SetBool("Open", true);
    }

    public void Deactivate()
    {
        _animator.SetBool("Open", false);
    }
}
