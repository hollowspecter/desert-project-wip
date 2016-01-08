using UnityEngine;
using System.Collections;
using System;

public class StampTool : ITool
{ 
    private RenderTexDrawing _map;

    private int selectedIndex = 0;
    float rotationSpeed = 40f;
    float scaleSpeed = 5f;
    private Sprite[] _images;
    private GameObject _stampPrefab;

    private Vector3 cursorPosition;
    private Quaternion cursorLocalRotation;
    private float cursorLocalScale;

    public StampTool(RenderTexDrawing map, Sprite[] images, GameObject stampPrefab)
    {
        _map = map;
        _images = images;
        _stampPrefab = stampPrefab;
    }

    public void Update(Vector3 cursorPosition, Quaternion cursorLocalRotation, float cursorLocalScale)
    {
        this.cursorPosition = cursorPosition;
        this.cursorLocalRotation = cursorLocalRotation;
        this.cursorLocalScale = cursorLocalScale * 0.15f;
    }

    public void ButtonA()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonADown()
    {
        StampSelectedImage();
    }

    public void ButtonAUp()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonB()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonBDown()
    {
        NextImage();
    }

    public void ButtonBUp()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonX()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonXDown()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void ButtonXUp()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    public void RightStick(float x, float y)
    {
        _map.RotateAndScaleCursor(x, y, rotationSpeed, scaleSpeed);
    }

    public void StampSelectedImage()
    {
        GameObject g = (GameObject)GameObject.Instantiate(_stampPrefab, cursorPosition, cursorLocalRotation);
        g.GetComponent<SpriteRenderer>().sprite = _images[selectedIndex];
        g.transform.SetParent(_map.transform);
        g.transform.localScale = new Vector3(cursorLocalScale, cursorLocalScale, cursorLocalScale);

        _map.CaptureRenderTex();
        GameObject.Destroy(g);
    }

    private void NextImage()
    {
        selectedIndex++;
        if (selectedIndex == _images.Length)
            selectedIndex = 0;
        _map.SetCursorImage(_images[selectedIndex]);
    }

    public void Activate()
    {
        _map.SetCursorImage(_images[selectedIndex]);
    }

    public void Deactivate()
    {
    }
}
