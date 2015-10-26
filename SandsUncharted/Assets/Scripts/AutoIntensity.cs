///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class AutoIntensity : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private Gradient nightDayColor;
    
    // Intensity
    [SerializeField]
    private float maxIntensity = 3f;
    [SerializeField]
    private float minIntensity = 0f;
    [SerializeField]
    private float minPoint = -.2f;

    // Ambient
    [SerializeField]
    private float maxAmbient = 1f;
    [SerializeField]
    private float minAmbient = 0f;
    [SerializeField]
    private float minAmbientPoint = -.2f;

    // Fog
    [SerializeField]
    private Gradient nightDayFogColor;
    [SerializeField]
    private AnimationCurve fogDensityCurve;
    [SerializeField]
    private float fogScale = 1f;

    // Atmosphere Thickness
    [SerializeField]
    private float dayAtmosphereThickness = .4f;
    [SerializeField]
    private float nightAtmosphereThickness = .87f;

    // Rotation
    [SerializeField]
    private Vector3 dayRotateSpeed;
    [SerializeField]
    private Vector3 nightRotateSpeed;

    //private global
    private float skySpeed = 1f;
    private Light mainLight;
    private Skybox sky;
    private Material skyMat;
    private bool isNight = false;
    #endregion

    #region Properties (public)
    public bool IsNight { get { return isNight; } }
    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {

    }

    void Update()
    {
        // Calculate and set Light Intensity
        float range = 1 - minPoint;
        float percentage = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / range);
        float intensity = ((maxIntensity - minIntensity) * percentage) + minIntensity;
        mainLight.intensity = intensity;
        
        // Calculate and set Ambient Intensity
        range = 1 - minAmbientPoint;
        percentage = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / range);
        intensity = ((maxAmbient - minAmbient) * percentage) + minAmbient;
        RenderSettings.ambientIntensity = intensity;

        // Set Ambient Color
        mainLight.color = nightDayColor.Evaluate(percentage);
        RenderSettings.ambientLight = mainLight.color;

        // Set Fog
        RenderSettings.fogColor = nightDayFogColor.Evaluate(percentage);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(percentage) * fogScale;

        // Set Atmosphere Thickness
        float thickness = ((dayAtmosphereThickness - nightAtmosphereThickness) * percentage) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", thickness);

        // Manually Speed Up Rotation
        if (Input.GetKeyDown(KeyCode.Q)) skySpeed *= .5f;
        if (Input.GetKeyDown(KeyCode.E)) skySpeed *= 2f;

        // Rotate
        if (percentage > 0) {
            isNight = false;
            transform.Rotate(dayRotateSpeed * Time.deltaTime * skySpeed);
        }
        else {
            isNight = true;
            transform.Rotate(nightRotateSpeed * Time.deltaTime * skySpeed);
        }
    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {

    }

    #endregion

    #region Methods

    #endregion
}