///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class LightSwitch : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private bool isLight = false;
    [SerializeField]
    private float lightingSpeed = 3f;

    private Light light;
    private float regularIntensity;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        light = transform.GetComponentInChildren<Light>();
        if (light == null) {
            Debug.LogError("Light source not found in children", this);
        }
        regularIntensity = light.intensity;
    }

    void Update()
    {
        if (isLight) {
            light.intensity = Mathf.Lerp(light.intensity, regularIntensity, lightingSpeed * Time.deltaTime);
        }
        else {
            light.intensity = Mathf.Lerp(light.intensity, 0f, lightingSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region Methods
    public void SwitchLight(bool lights){
        isLight = lights;
    }
    #endregion
}