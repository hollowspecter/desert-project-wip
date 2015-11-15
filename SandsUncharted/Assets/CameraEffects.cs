using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraEffects : MonoBehaviour
{
    private Bloom bloom;
    private VignetteAndChromaticAberration vignette;

    void Awake()
    {
        bloom = GetComponent<Bloom>();
        Assert.IsNotNull<Bloom>(bloom);

        vignette = GetComponent<VignetteAndChromaticAberration>();
        Assert.IsNotNull<VignetteAndChromaticAberration>(vignette);
    }

    public void Toggle()
    {
        bloom.enabled = !bloom.enabled;
        vignette.enabled = !vignette.enabled;
    }
}
