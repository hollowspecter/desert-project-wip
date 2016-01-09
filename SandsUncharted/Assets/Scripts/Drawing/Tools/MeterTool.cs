using UnityEngine;
using System.Collections;

public class MeterTool : ITool
{
    private RenderTexDrawing map;

    private MeshLine line;

    private Vector3 cursorPosition;

    private Sprite cursor;

    public MeterTool(RenderTexDrawing map, MeshLine line, Sprite cursor)
    {
        this.map = map;
        this.line = line;
        this.cursor = cursor;
    }

    public void Update(Vector3 cursorPosition, Quaternion cursorLocalRotation, float cursorLocalScale)
    {
        this.cursorPosition = cursorPosition;
    }

    public void Activate()
    {
        map.ResetCursorRotScal();
        line.SetLineOffsetFactor(0);
        map.SetCursorImage(cursor);
    }

    public void Deactivate()
    {

    }

    public void ButtonADown()
    {
        line.SetStart(cursorPosition);
    }

    public void ButtonAUp()
    {
        line.ClearPoints();
        line.ClearMesh();
    }

    public void ButtonA()
    {
        line.SetEnd(cursorPosition);
    }

    public void ButtonBDown()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void ButtonBUp()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void ButtonB()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void ButtonXDown()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void ButtonXUp()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void ButtonX()
    {
        Debug.Log("No Buttonfunction implemented");
    }

    public void RightStick(float x, float y)
    {
        Debug.Log("No Buttonfunction implemented");
    }
}
