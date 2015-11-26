using UnityEngine;
using System.Collections;

[System.Serializable]
public class NoiseLayer
{
    /* Enums */
    public enum NoiseOperators
    {
        Add,
        Subtract
    }

    #region private Attributes

    [SerializeField]
    private string LayerName = "Default";

    [Tooltip("Frequency")]
    [SerializeField]
    private float frequency = 1f;

    [Tooltip("Amplitude")]
    [SerializeField]
    private float amplitude = 1f;

    [Tooltip("Octaves. The noise will automatically generate and sum the octaves layers."+
        "One layer is achieved by multiplying the frequency by the lacunarity (increasing),"+
        " and multiplying the amplitude by the persistence (decreasing")]
    [Range(1, 8)]
    [SerializeField]
    private int octaves = 1;

    [Tooltip("The value by which the frequency gets multiplied for each octave. Usually 2.")]
    [Range(1f, 4f)]
    [SerializeField]
    private float lacunarity = 2f;

    [Tooltip("The value by which the persistence gets multiplied for each octave. Usually 0.5")]
    [Range(0f, 1f)]
    [SerializeField]
    private float persistence = 0.5f;

    [Tooltip("1D, 2D or 3D Noise")]
    [Range(1, 3)]
    [SerializeField]
    private int dimension = 3;

    [SerializeField]
    private NoiseMethodType type = NoiseMethodType.Perlin;

    [SerializeField]
    private NoiseOperators operation = NoiseOperators.Add;

    [SerializeField]
    private bool active = true;

    #endregion

    #region public Properties

    public NoiseOperators Operation { get { return operation; } }

    #endregion

    /* Methods */

    public float getValue(Vector3 point)
    {
        NoiseMethod method = Noise.methods[(int)type][dimension - 1];
        return Noise.Sum(method, point, frequency, octaves, lacunarity, persistence) * amplitude;
    }
}