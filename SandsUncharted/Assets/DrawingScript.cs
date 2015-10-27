using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawingScript : MonoBehaviour 
{
	public Texture2D brush;
	public GameObject brushCircle;

    Texture2D texture;
    public Texture2D saveTexture;
    Camera cam;

	bool first;
	Vector2 oldUV;
	Vector2 pixelUV;

    float reticleSpeed = 0.8f;

	float paintinterval = 2f; // how many pixels between two drawpositions

	Color[] brushPx;

	Color[] currentPx;
	Color[] scaledBrushPx;
	int scaledBrushW;
	int scaledBrushH;
	float scaleFactor = 1f;
	float scaleSpeed = 1f;

	MeshRenderer mesh;
	float borderWidth = 0.1f;

	void Start() 
	{
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		texture = new Texture2D(1024,1024, TextureFormat.ARGB32, false);

		GetComponent<MeshRenderer> ().material.SetTexture(0, texture);
		brushPx = brush.GetPixels ();
		oldUV = pixelUV = new Vector2 ();
		first = true;
		mesh = GetComponent<MeshRenderer> ();
	}

	void Update() 
	{
        RaycastHit mouseHit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast (ray, out mouseHit))
        {
            //brushCircle.transform.position = mouseHit.point + transform.up * 0.01f;
        }

        Debug.DrawLine(ray.origin, mouseHit.point, Color.red);

        /* get drawposition from Mouse */
		if (Input.GetMouseButton(0)) 
		{			
			if (mouseHit.collider ==  null)
				return;

			oldUV = pixelUV;
		
			pixelUV = mouseHit.textureCoord;

			pixelUV.x *= texture.width;
			pixelUV.y *= texture.height;
			if(first)
			{
				oldUV = pixelUV;
				first = false;
			}
			//Debug.Log ("old: " + oldUV.ToString() + "| new: " + pixelUV.ToString());
	
            Draw(oldUV, pixelUV);
		}


        /****move Reticle with joystick****/
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 axisVector = new Vector3(h, 0, v);
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();
        brushCircle.transform.position += reticleSpeed * reticleSpeed * axisVector * Time.deltaTime;


		brushCircle.transform.position = new Vector3 (Mathf.Clamp (brushCircle.transform.position.x, mesh.bounds.min.x + borderWidth, mesh.bounds.max.x - borderWidth),
		                                             brushCircle.transform.position.y,
		                                             Mathf.Clamp (brushCircle.transform.position.z, mesh.bounds.min.z + borderWidth, mesh.bounds.max.z - borderWidth));
		 
        /* get  drawPosition from brushreticle*/

        if(Input.GetButton("A"))
        {
            RaycastHit reticleHit;
            if (Physics.Raycast(new Ray(brushCircle.transform.position, -brushCircle.transform.up), out reticleHit))
            {
                oldUV = pixelUV;
                pixelUV = reticleHit.textureCoord;

                pixelUV.x *= texture.width;
			    pixelUV.y *= texture.height;
			    if(first)
			    {
					Debug.Log ("first");
				    oldUV = pixelUV;
				    first = false;
			    }

                Draw(oldUV, pixelUV);
            }

        }

		float scaleAxis = Input.GetAxis ("RightStickX");
		if (Mathf.Abs (scaleAxis) > 0.2) 
		{
			if(scaleAxis < 0)
			{
				scaleFactor = Mathf.Clamp(scaleFactor + (-1.0f * scaleSpeed * Time.deltaTime), 0.3f, 3f);
			}
			else
			{
				scaleFactor = Mathf.Clamp(scaleFactor + (1.0f * scaleSpeed * Time.deltaTime), 0.3f, 3f);
			}

			brushCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		}


        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("A")) 
		{
			first = true;

		}
	}

    
    void Draw(Vector2 OldUV, Vector2 NewUV)
    {
        Vector2[] positions = FindStampPositions(OldUV, NewUV);
        //Debug.Log (positions.Length.ToString ());
        for (int i = 0; i < positions.Length; ++i)
        {
            Vector2 p = positions[i];
            if (p.x >= 0 && p.x + brush.width <= texture.width && p.y >= 0 && p.y + brush.height <= texture.height)
                Stamp((int)p.x - scaledBrushW/2, (int)p.y - scaledBrushH/2);
        }
        texture.Apply(true);
        //saveTexture.SetPixels(texture.GetPixels());
        //saveTexture.Apply();
    }
	
	
	Vector2[] FindStampPositions(Vector2 v1, Vector2 v2)
	{
		
		Vector2 dir = (v2 - v1);
		Vector2[] tmp = new Vector2[(int)(Mathf.Max(dir.magnitude / paintinterval,1f))];
		//Debug.Log ("v1: " + v1.ToString() + " v2 : " + v2.ToString ());
		//Debug.Log ("length between inputpoints: " + dir.magnitude + "interpolated input points: " + tmp.Length);
		for (int i = 0; i < tmp.Length; ++i) 
		{
			//interpolate the points between the first and last position by moving the position paintinterval px to the next position
			tmp[i] = v1 + i * dir.normalized * paintinterval;
		}
		
		return tmp;
	}
	
	
	void Stamp(int x, int y)
	{
		if (scaleFactor * brush.width != scaledBrushW) 
		{
			brushRescale(scaleFactor);
		}

		UpdateScaledBrush(x, y);
		texture.SetPixels(x, y, scaledBrushW, scaledBrushH, currentPx);
	}


	void UpdateScaledBrush(int x, int y)
	{
		for (int i = 0; i < scaledBrushW; i++) 
		{
			for (int j = 0; j < scaledBrushH; j++)
			{
				Color c1= scaledBrushPx[i + scaledBrushW * j];
				if(c1.a != 1.0f)
				{
					currentPx[scaledBrushW * j + i] =  Color.Lerp(texture.GetPixel(x + i, y + j), c1, c1.a);
					//Debug.Log (scaledBrushPx[i + scaledBrushW * j].ToString());
				}
			}
		}
	}

	void UpdateBrush(int x, int y)
	{
		for (int i = 0; i < brush.width; ++i) 
		{
			for (int j = 0; j < brush.height; ++j)
			{
				Color c1= brush.GetPixel(i, j);
				if(c1.a != 1.0f)
				{
					brushPx[brush.width * j + i] =  Color.Lerp(texture.GetPixel(x + i, y + j), c1, c1.a);
				}
			}
		}
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(brushCircle.transform.position, -brushCircle.transform.up);
        
    }

	void brushRescale(float factor)
	{
		scaledBrushW = (int) (brush.width * factor);
		scaledBrushH = (int) (brush.height * factor);

		scaledBrushPx = new Color[scaledBrushW * scaledBrushH];
		currentPx = new Color[scaledBrushW * scaledBrushH];
		//Nearest Neighbor testing
		for (int x = 0; x < scaledBrushW; x++) 
		{
			for(int y = 0; y < scaledBrushW; y++)
			{
				//Debug.Log ("newX/Y: " + x + "/" + y + " oldX/Y: " + Mathf.RoundToInt(x/factor) + "/" + Mathf.RoundToInt(y/factor));
				scaledBrushPx[x + scaledBrushW * y] = brush.GetPixel(Mathf.RoundToInt(x/factor), Mathf.RoundToInt(y/factor));
			}
		}
	}

}
