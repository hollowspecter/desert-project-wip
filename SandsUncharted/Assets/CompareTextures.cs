///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class CompareTextures : MonoBehaviour
{
    #region variables (private)
    [SerializeField]
    private Texture2D solutionTexture;
    [SerializeField]
    private Text[] textQuarters;
    [SerializeField]
    private float[] passingMinRatio = new float[4];
    [SerializeField]
    private float[] passingMinMatchingRatio = new float[4];
    [SerializeField]
    private float[] averageRatios;
    [SerializeField]
    private float[] averageMatchingRatios;

    private List<float>[] blackRatios;
    private List<float>[] matchingRatios;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        blackRatios = new List<float>[4];
        matchingRatios = new List<float>[4];
        for (int i = 0; i < 4; ++i) {
            blackRatios[i] = new List<float>();
            matchingRatios[i] = new List<float>();
        }

        averageRatios = new float[4];
        averageMatchingRatios = new float[4];
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {

    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {

    }

    #endregion

    #region Methods
    /// <summary>
    /// Divides both textures in 4 quarters, then compares how many
    /// of the textures black pixels are also black on the solution
    /// texture.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="perfectTexture"></param>
    public bool CompareTextureToPerfect(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        Color[] perfPxl = solutionTexture.GetPixels();

        int[] blackPixelsTexture = new int[] { 0, 0, 0, 0 };
        int[] blackPixelsPerfect = new int[] { 0, 0, 0, 0 };
        int[] matchingPixels = new int[] { 0, 0, 0, 0 };
        int[] length = new int[]{0,0,0,0};
        float[] blackratioTexture = new float[4];
        float[] blackratioPerfect = new float[4];
        float[] matchingRatio = new float[4];

        string txt = "Length of Pefect: " + perfPxl.Length + ";\n" +
            "Length of the Try: " + pixels.Length + "\n";

        // Check if length is similar
        if (pixels.Length != perfPxl.Length)
            Debug.LogError("The Textures are not of the same size");

        // Going through all the pixels
        for (int u = 0; u < texture.width; ++u) {
            for (int v = 0; v < texture.height; ++v) {

                // Get the value in the Array from U and V
                int uv = u + v*1024;

                // First check, which Quarter it belongs to
                int quarter = 0;
                if (u < texture.width / 2 && v < texture.height / 2) // first quarter
                    quarter = 0;
                else if (u > texture.width / 2 && v < texture.height / 2) // second quarter
                    quarter = 1;
                else if (u > texture.width / 2 && v > texture.height / 2) // third quarter
                    quarter = 2;
                else if (u < texture.width / 2 && v > texture.height / 2) //fourth quarter
                    quarter = 3;

                // Count the pixels in one quarter to be secure
                length[quarter]++;

                // Then check for blackness
                bool blackTexture = false;
                bool blackPerfect = false;

                if (IsBlack(pixels[uv])) {
                    blackPixelsTexture[quarter]++;
                    blackTexture = true;
                }
                if (IsBlack(perfPxl[uv])) {
                    blackPixelsPerfect[quarter]++;
                    blackPerfect = true;
                }

                // Check if Texture is matching the Perfect pixel in case of black
                if (blackTexture && blackPerfect)
                    matchingPixels[quarter]++;
            }
        }

        // Calculate the data for the quarters
        for (int i = 0; i < 4; ++i) {
            blackratioTexture[i] = ((float)blackPixelsTexture[i]) / ((float)length[i]);
            blackratioPerfect[i] = ((float)blackPixelsPerfect[i]) / ((float)length[i]);
            matchingRatio[i] = ((float)matchingPixels[i]) / ((float)blackPixelsTexture[i]);
        }

        // Generate Debug Messages if the Debug Texts are initialised
        if (textQuarters.Length > 0) {
            for (int i = 0; i < 4; ++i) {
                string text = "Drawn Texture has " + blackPixelsTexture[i] + "px => " + blackratioTexture[i] * 100 + "%\n";
                text += "Perfect Texture has " + blackPixelsPerfect[i] + "px => " + blackratioPerfect[i] * 100 + "%\n";
                text += "Matching Pixels: " + matchingPixels[i] + " => " + matchingRatio[i] * 100 + "% Hit Rate\n";
                textQuarters[i].text = text;
            }
        }

        // Add the Data to the Lists to calculte Averages
        for (int i = 0; i < 4; ++i) {
            blackRatios[i].Add(blackratioTexture[i]);
            matchingRatios[i].Add(matchingRatio[i]);
        }
        CalculateAverages();

        // Return result
        bool result = true;
        for (int i = 0; i < 4; ++i) {
            
            if (blackPixelsTexture[i] < passingMinRatio[i] || // check if Black Ratio passes
                matchingRatio[i] < passingMinMatchingRatio[i]) { // check if Matching Ratio passes (Hit rate)
                result = false;
                break;
            }
        }
        return result;
    }

    private void CalculateAverages()
    {
        for (int i=0; i<4; ++i){
            // black averages
            float tmp = 0;
            foreach (float a in blackRatios[i]) {
                tmp += a;
            }
            averageRatios[i] = tmp / (float)blackRatios[i].Count;

            // matching averages
            tmp = 0;
            foreach (float a in matchingRatios[i]) {
                tmp += a;
            }
            averageMatchingRatios[i] = tmp / (float)matchingRatios[i].Count;
        }
    }

    private bool IsBlack(Color c)
    {
        return c.r < .4f && c.g < .4f && c.b < .4f && c.a > 0;
    }
    #endregion
}