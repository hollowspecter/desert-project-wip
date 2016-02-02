﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

/// <summary>
/// Derive from this and override the action function!
/// </summary>
public class RadialMenuItem : MonoBehaviour
{
    private Image selectedImage;

    void Awake()
    {
        selectedImage = transform.FindChild("selected").GetComponent<Image>();
        Assert.IsNotNull<Image>(selectedImage);
        selectedImage.color = Color.clear;
    }

    public void Toggle(bool b)
    {
        if (b) {
            selectedImage.color = Color.white;
        }
        else {
            selectedImage.color = Color.clear;
        }
    }

    public void Action()
    {
        Debug.Log("Action called!", this);
    }
}