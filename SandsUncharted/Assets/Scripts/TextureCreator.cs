using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureCreator : MonoBehaviour
{
    [SerializeField]
	[Range(2, 512)]
	private int resolution = 256;
    [SerializeField]
    private Gradient coloring;

	private Texture2D texture;
	
	public void FillTexture (MapGenerator mapgenScript, bool single, int index) {
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
		
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++) {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);

                // if just a single noise
                float sample;
                if (single)
                    sample = mapgenScript.noises[index].getValue(point);
                else
                    sample = mapgenScript.GetValueFromNoises(point);

				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}
		texture.Apply();

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        // For testing purposes, also write to a file in the project folder
        string path = EditorUtility.SaveFilePanel("Save Visualizing Noise Texture", Application.dataPath, "noiseVisualization.png", "png");
        if (path.Length > 0)
            File.WriteAllBytes(path , bytes);
	}
}