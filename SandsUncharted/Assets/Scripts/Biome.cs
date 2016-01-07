using UnityEngine;
using System.Collections;

public class Biome : MonoBehaviour
{
    [SerializeField]
    private float heightOffset = 0f;
    [SerializeField]
    private NoiseLayer[] noises;

    public float GetValueFromNoises(Vector3 point)
    {
        float value = NoiseLayer.getValueFromNoises(ref noises, point);
        return value - heightOffset;
    }

    public float GetValueFromNoises(float x, float y, float z)
    {
        return GetValueFromNoises(new Vector3(x, y, z));
    }
}
