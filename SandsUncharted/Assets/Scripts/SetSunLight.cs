///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class SetSunLight : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private Renderer lightWall;
    [SerializeField]
    private Renderer water;
    [SerializeField]
    private Transform stars;
    [SerializeField]
    private Transform worldProbe;
    [SerializeField]
    private float maxEmissionIntensity = 5f;

    private Transform _transform;
    private bool lightOn;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        _transform = transform;
        lightOn = false;
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {

    }

    /// <summary>
    /// Gets updated every frame
    /// </summary>
    void Update()
    {
        stars.transform.rotation = _transform.rotation;

        if (Input.GetKeyDown(KeyCode.T)) {
            lightOn = !lightOn;
        }

        if (lightOn) {
            Color final = Color.white * Mathf.LinearToGammaSpace(maxEmissionIntensity);
            lightWall.material.SetColor("_EmissionColor", final);
            DynamicGI.SetEmissive(lightWall, final);
        }
        else {
            Color final = Color.white * Mathf.LinearToGammaSpace(0);
            lightWall.material.SetColor("_EmissionColor", final);
            DynamicGI.SetEmissive(lightWall, final);
        }

        Vector3 tvec = Camera.main.transform.position;
        worldProbe.transform.position = tvec;

        water.material.mainTextureOffset = new Vector2(Time.time / 100f, 0);
        water.material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, Time.time / 80f));
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