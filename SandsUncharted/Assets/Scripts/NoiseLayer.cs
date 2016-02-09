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

#pragma warning disable 0414
    [SerializeField]
    private string LayerName = "Default";
#pragma warning restore 0414

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

    [Tooltip("The final Noise is a weighted sum of the several noise layers. Every Noiselayer will be multiplied by its weight.")]
    [SerializeField]
    private float weight = 1f;

    [Tooltip("Offset to which the points will be sampled. Basically \"moves\" the noise around.")]
    [SerializeField]
    private Vector3 offsetPosition = new Vector3();

    [Tooltip("Rotationoffset to which the points will be sampled. Basically \"rotates\" the noise around its origin.")]
    [SerializeField]
    private Vector3 offsetRotation = new Vector3();

    #endregion

    #region public Properties

    public NoiseOperators Operation { get { return operation; } }
    public bool Active{ get { return active; } }
    public float Weight { get { return weight; } }

    #endregion

    /* Methods */

    public void setOffsetPosition(Vector3 pos)
    {
        offsetPosition = pos;
    }

    public void setOffsetRotation(Vector3 euler)
    {
        offsetRotation = euler;
    }

    public float getValue(Vector3 p)
    {
        Vector3 point = p;
        point += offsetPosition;
        point = Quaternion.Euler(offsetRotation) * point;
        NoiseMethod method = Noise.methods[(int)type][dimension - 1];
        return Noise.Sum(method, point, frequency, octaves, lacunarity, persistence) * amplitude;
    }

    public static float getValueFromNoises(ref NoiseLayer[] noises, Vector3 point)
    {
        float value = 0;
        for (int i = 0; i < noises.Length; ++i) {
            // Check if active
            if (!noises[i].Active)
                continue;

            // Check the operation and act accordingly
            NoiseLayer.NoiseOperators op = noises[i].Operation;
            switch (op) {
                case NoiseLayer.NoiseOperators.Add:
                    value += noises[i].getValue(point) * noises[i].Weight;
                    break;
                case NoiseLayer.NoiseOperators.Subtract:
                    value -= noises[i].getValue(point) * noises[i].Weight;
                    break;
            }
        }
        return value;
    }
}