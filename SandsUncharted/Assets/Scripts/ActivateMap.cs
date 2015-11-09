///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class ActivateMap : MonoBehaviour
{
    private GameObject map;

    void Awake()
    {
        map = transform.FindChild("MapAndBrush").gameObject;
        if (map == null) {
            Debug.LogError("Map and Brush not found", this);
        }
    }

    void OnActivateMap()
    {
        map.SetActive(true);
    }

    void OnDeactivateMap()
    {
        map.SetActive(false);
    }

    void OnEnable()
    {
        MapState.OnDrawEnter += OnActivateMap;
        MapState.OnDrawExit += OnDeactivateMap;
    }

    void OnDisable()
    {
        MapState.OnDrawEnter -= OnActivateMap;
        MapState.OnDrawExit -= OnDeactivateMap;
    }
}