///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public abstract class State : MonoBehaviour
{
    protected GameManager gameManager;

    [HideInInspector]
    public bool Active;

    abstract public void EnterState();

    abstract public void ExitState();
}