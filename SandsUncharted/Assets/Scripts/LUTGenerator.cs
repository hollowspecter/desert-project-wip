using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class LUTGenerator : MonoBehaviour
{
    [SerializeField]
    [Range(2, 512)]
    private int resolution = 256;

    [SerializeField]
    private float minAngleForSand = 30f;
    [SerializeField]
    private float maxAngleForSand = 120f;
    
    private Texture2D texture;

    public void FillTexture()
    {
        if (texture == null) {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 9;
        }

        if (texture.width != resolution) {
            texture.Resize(resolution, resolution);
        }

        /*
         * Set the pixels here
         */
        // U = Horizontal = Angle mapped from 0 to 1 as 0°-180° of Slope
        for (int u = 0; u < resolution; ++u) {
            // V = Vertical = Height mapped from 0 to 1 as 0 - MapGen.Height*Chunksize
            for (int v = 0; v < resolution; ++v) {

                // Red is sand, Blue is rock
                float uPercentage = ((float)u) / ((float)resolution);
                float vPercentage = ((float)v) / ((float)resolution);

                float minSandPercentage = ((float)minAngleForSand) / 180f;
                float maxSandPercentage = ((float)maxAngleForSand) / 180f;

                if (uPercentage >= minSandPercentage && uPercentage < maxSandPercentage) {
                    texture.SetPixel(u, v, Color.red);
                }
                else
                    texture.SetPixel(u, v, Color.green);
            }
        }

        // Apply to the texture
        texture.Apply();

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        // For testing purposes, also write to a file in the project folder
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save Visualizing Noise Texture", Application.dataPath, "LUT.png", "png");
        if (path.Length > 0)
            System.IO.File.WriteAllBytes(path, bytes);
    }
}
