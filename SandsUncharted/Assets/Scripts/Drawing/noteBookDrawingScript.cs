using UnityEngine;
using System.Collections;

public class noteBookDrawingScript : DrawingScript
{
    [SerializeField]
    private bool isLeftPage = false;

    private Animator _anim;

    protected override void OnOverrideEnable()
    {
        DrawState.MoveCursor += ReceiveLeftStickInput;
        DrawState.Draw += OnDraw;
        DrawState.Erase += OnErase;
        DrawState.OnDrawExit += OnDrawExit;
        DrawState.Clear += OnClear;
        DrawState.LiftedPen += OnLiftedPen;
    }

    protected override void OnOverrideDisable()
    {
        DrawState.MoveCursor -= ReceiveLeftStickInput;
        DrawState.Draw -= OnDraw;
        DrawState.Erase -= OnErase;
        DrawState.OnDrawExit -= OnDrawExit;
        DrawState.Clear -= OnClear;
        DrawState.LiftedPen -= OnLiftedPen;
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
