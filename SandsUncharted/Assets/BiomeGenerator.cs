using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections;

public class BiomeGenerator : MonoBehaviour
{
    [SerializeField]
    private string seed;
    [SerializeField]
    private NoiseLayer[] redNoise;
    [SerializeField]
    private NoiseLayer[] greenNoise;
    [SerializeField]
    private NoiseLayer[] blueNoise;

    private MapGenerator mapgen;


    public void GenerateBiomeMap()
    {
        // Fetch Data
        mapgen = GetComponent<MapGenerator>();
        Assert.IsNotNull<MapGenerator>(mapgen);

        // Prepare the noise data
        RandomiseNoiseLayers();

        // Create a new texture
        Texture2D texture = new Texture2D(mapgen.TotalWidth, mapgen.TotalDepth,
            TextureFormat.RGB24, true);
        texture.name = "generatedBiomeMap";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Trilinear;
        texture.anisoLevel = 9;
        

        // fill in the values
        Vector3[,] values = new Vector3[mapgen.TotalWidth, mapgen.TotalDepth];
        float redMin = 999f;
        float greenMin = 999f;
        float blueMin = 999f;
        float redMax = float.MinValue;
        float greenMax = float.MinValue;
        float blueMax = float.MinValue;

        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {
                // retrieve values
                float redValue = NoiseLayer.getValueFromNoises(ref redNoise, new Vector3(x, y, 0));
                float greenValue = NoiseLayer.getValueFromNoises(ref greenNoise, new Vector3(x, y, 0));
                float blueValue = NoiseLayer.getValueFromNoises(ref blueNoise, new Vector3(x, y, 0));
                values[x, y] = new Vector3(redValue, greenValue, blueValue);

                // check min and max conditions
                if (redValue < redMin)
                    redMin = redValue;
                else if (redValue > redMax)
                    redMax = redValue;

                if (greenValue < greenMin)
                    greenMin = greenValue;
                else if (greenValue > greenMax)
                    greenMax = greenValue;

                if (blueValue < blueMin)
                    blueMin = blueValue;
                else if (blueValue > blueMax)
                    blueMax = redValue;
            }
        }

        // normalize each value between 0 and 1
        float redMaxDiff = redMax - redMin;
        float greenMaxDiff = greenMax - greenMin;
        float blueMaxDiff = blueMax - blueMin;
        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {

                // normalize red value
                float redValue = values[x, y].x - redMin;
                float percentage = redValue / redMaxDiff;
                values[x, y].x = percentage;

                // normalize green value
                float greenValue = values[x, y].y - greenMin;
                percentage = greenValue / greenMaxDiff;
                values[x, y].y = percentage;

                // normalize blue value
                float blueValue = values[x, y].z - blueMin;
                percentage = blueValue / blueMaxDiff;
                values[x, y].z = percentage;
            }
        }

        // Apply the pixels
        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {
                texture.SetPixel(x, y,
                    new Color(values[x, y].x,
                    values[x, y].y,
                    values[x, y].z));
            }
        }
        texture.Apply();

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        // Write to a file in the project folder
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save Biome Texture",
            Application.dataPath, "biomeTexture.png", "png");
        if (path.Length > 0)
            System.IO.File.WriteAllBytes(path, bytes);

        var tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (tImporter != null) {
            tImporter.textureType = TextureImporterType.Advanced;

            tImporter.isReadable = true;

            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }

    public void RandomiseNoiseLayers()
    {
        Random.seed = seed.GetHashCode();

        NoiseLayer[][] noises = new NoiseLayer[3][];
        noises[0] = redNoise;
        noises[1] = greenNoise;
        noises[2] = blueNoise;

        for (int i = 0; i < noises.GetLength(0); ++i) {
            for (int j = 0; j < noises[i].GetLength(0); ++j) {
                Vector3 position = randomVector(-100f, 100f);
                Vector3 rotation = randomVector(0f, 360f);
                noises[i][j].setOffsetPosition(position);
                noises[i][j].setOffsetRotation(rotation);
            }
        }
    }

    Vector3 randomVector(float min, float max)
    {
        float x = Random.Range(min, max);
        float y = Random.Range(min, max);
        float z = Random.Range(min, max);
        return new Vector3(x, y, z);
    }
}


