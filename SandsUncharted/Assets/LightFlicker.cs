///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class LightFlicker : MonoBehaviour
{
//    var minFlickerIntensity : float = 0.5;
// var maxFlickerIntensity : float = 2.5;
// var flickerSpeed : float = 0.035;
// 
// private var randomizer : int = 0;
//  
//  while (true)
//  {
//     if (randomizer == 0) {
//       light.intensity = (Random.Range (minFlickerIntensity, maxFlickerIntensity));
// 
//     }
//     else light.intensity = (Random.Range (minFlickerIntensity, maxFlickerIntensity));
// 
//     randomizer = Random.Range (0, 1.1);
//     yield WaitForSeconds (flickerSpeed);
// }

    #region variables (private)
    [SerializeField]
    private float minFlickerIntensity = 0.5f;
    [SerializeField]
    private float maxFlickerIntensity = 2.5f;
    [SerializeField]
    private Gradient colorChange;

    private Light _light;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        float percentage = Mathf.PerlinNoise(0, Time.time*2f);
        _light.color = colorChange.Evaluate(percentage);
        _light.intensity = Mathf.Lerp(minFlickerIntensity, maxFlickerIntensity, percentage);
    }

    #endregion

    #region Methods

    #endregion
}