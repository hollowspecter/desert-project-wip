using UnityEngine;
using System.Collections;

public class noteBookDrawingScript : DrawingScript
{
    [SerializeField]
    private bool isLeftPage = false;

    private Animator _anim;

    protected override void OnOverrideEnable()
    {
        NotebookState.MoveCursor += ReceiveLeftStickInput;
        NotebookState.Draw += OnDraw;
        NotebookState.Erase += OnErase;
        NotebookState.OnNotebookExit += OnDrawExit;
        NotebookState.Clear += OnClear;
        NotebookState.LiftedPen += OnLiftedPen;
    }

    protected override void OnOverrideDisable()
    {
        NotebookState.MoveCursor -= ReceiveLeftStickInput;
        NotebookState.Draw -= OnDraw;
        NotebookState.Erase -= OnErase;
        NotebookState.OnNotebookExit -= OnDrawExit;
        NotebookState.Clear -= OnClear;
        NotebookState.LiftedPen -= OnLiftedPen;
    }

    void OnEnable()
    {
        OnOverrideEnable();
        transform.parent.GetComponent<Animator>().SetBool("IsLeftPage", isLeftPage);
    }

    void OnDisable()
    {
        OnOverrideDisable();
        isLeftPage = transform.parent.GetComponent<Animator>().GetBool("IsLeftPage");
    }

    public void SetIsLeftPage(bool b)
    {
        isLeftPage = b;
    }

    public bool GetIsLeftPage()
    {
        return isLeftPage;
    }
}
