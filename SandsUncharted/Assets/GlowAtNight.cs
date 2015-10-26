///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class GlowAtNight : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private float minGlowIntensity = 0f;
    [SerializeField]
    private float maxGlowIntensity = 1f;
    [SerializeField]
    private float glowSpeed = 1f;

    private AutoIntensity sun;
    private Material _material;
    private Renderer _renderer;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<AutoIntensity>();
        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {

    }

    void Update()
    {
        if (sun.IsNight) {
            Color final = Color.white * Mathf.LinearToGammaSpace(maxGlowIntensity);
            _material.SetColor("_EmissionColor", final);
            DynamicGI.SetEmissive(_renderer, final);
        }
        else {
            Color final = Color.white * Mathf.LinearToGammaSpace(0);
            _material.SetColor("_EmissionColor", final);
            DynamicGI.SetEmissive(_renderer, final);
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