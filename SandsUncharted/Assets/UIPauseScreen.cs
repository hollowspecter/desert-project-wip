///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class UIPauseScreen : MonoBehaviour
{
    #region variables (private)
    private GameObject pausePanel;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        pausePanel = transform.GetChild(0).gameObject;
        if (pausePanel == null) {
            Debug.LogError("Pause Panel has not been found. Please set the pausePanel as the first child of the PauseScreen Panel.", this);
        }
    }

    #endregion

    #region Methods

    void Pause()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    void Unpause()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    void OnEnable()
    {
        PauseState.Pause += Pause;
        PauseState.Unpause += Unpause;
    }

    void OnDisable()
    {
        PauseState.Pause -= Pause;
        PauseState.Unpause -= Unpause;
    }

    #endregion
}