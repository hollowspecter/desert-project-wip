using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawingScript : MonoBehaviour 
{
	public Texture2D brush;
	public GameObject brushCircle;

    Texture2D texture;
	Color clearColor;
    Camera cam;

	bool first;
	Vector2 oldUV;
	Vector2 pixelUV;

    float reticleSpeed = 0.6f;
	Vector3 speed = new Vector3();

	float paintinterval = 1f; // how many pixels between two drawpositions

	Color[] brushPx;
	Color[] currentPx;
	Color[] scaledBrushPx;
	int scaledBrushW;
	int scaledBrushH;
	float scaleFactor = 0.5f;
	float setScale = 0.5f;
	float scaleSpeed = 1f;
	float minScale = 0.25f;
	float maxScale = 1f;

	float minPX = 16f;
	float maxPX = 64f;

	MeshRenderer mesh;
	float meshWidth;
	float meshHeight;
	float border = 0.05f;

	Transform maxCorner;
	Transform minCorner;

	void Awake() 
	{
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		texture = new Texture2D(512,512, TextureFormat.ARGB32, false);
		clearColor  = new Color(0f,0f,0f,0f);
		floodTexture (clearColor);
		mesh = GetComponent<MeshRenderer> ();
		if (mesh == null) 
		{
			Debug.LogError("no mesh");
		}
		mesh.material.SetTexture("_DrawTex", texture);

		meshWidth = mesh.bounds.size.x;
		meshHeight = mesh.bounds.size.z;

		maxCorner = transform.parent.FindChild("rightEdge");
		minCorner = transform.parent.FindChild("topEdge");

		brushPx = brush.GetPixels ();
		oldUV = pixelUV = new Vector2 ();
		first = true;
		brushCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		brushRescale (scaleFactor);
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

        /****move Reticle with joystick****/
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 axisVector = h * transform.right + v * transform.forward;
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();
		float acc = 1.25f;
		Debug.DrawRay (brushCircle.transform.position, axisVector);
		speed += acc * axisVector * Time.deltaTime;

		brushCircle.transform.position += speed * Time.deltaTime;

		speed *= 0.75f;//friction

		//brushCircle.transform.position += (reticleSpeed * reticleSpeed) * axisVector * Time.deltaTime; //squared speed to get a better controllable curve

		if (mesh == null) 
		{
			Debug.LogError("no mesh in update");
		}
		//clamp brushposition to mapborder
		brushCircle.transform.localPosition = new Vector3 (Mathf.Clamp (brushCircle.transform.localPosition.x, minCorner.localPosition.x + border, maxCorner.localPosition.x - border), brushCircle.transform.localPosition.y, Mathf.Clamp(brushCircle.transform.localPosition.z, minCorner.localPosition.z + border, maxCorner.localPosition.z - border));


		/* draw on buttonpressfrom brushreticle*/
        if(Input.GetButton("A"))
        {
            RaycastHit reticleHit;
            if (Physics.Raycast(new Ray(brushCircle.transform.position, -brushCircle.transform.up), out reticleHit))
            {
                oldUV = pixelUV;
                pixelUV = reticleHit.textureCoord;
				//Debug.Log (reticleHit.textureCoord);

                pixelUV.x *= texture.width;
			    pixelUV.y *= texture.height;
			    if(first)
			    {
					Debug.Log ("first");
				    oldUV = pixelUV;
				    first = false;
					brushRescale (scaleFactor);
			    }

                Draw(oldUV, pixelUV);
            }
			else
			{
				Debug.LogError("Raycast did not hit!");
			}

        }

		else if (Input.GetButton ("B")) 
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
					brushRescale (scaleFactor);
				}
				
				Erase(oldUV, pixelUV);
			}
		}

		/* free brush scaling */

		float scaleAxis = Input.GetAxis ("RightStickX");
		if (Mathf.Abs (scaleAxis) > 0.2) 
		{
			if(scaleAxis < 0)
			{
				scaleFactor = Mathf.Clamp(scaleFactor + (-1.0f * scaleSpeed * Time.deltaTime), minPX/brush.width, maxPX/brush.width);
				setScale = scaleFactor;
			}
			else
			{
				scaleFactor = Mathf.Clamp(scaleFactor + (1.0f * scaleSpeed * Time.deltaTime), minPX/brush.width, maxPX/brush.width);
				setScale = scaleFactor;
			}

			brushCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		}

		//rescale based on speed slow = big / fast = small
		scaleFactor = Mathf.Clamp(scaleFactor + (((1f - axisVector.magnitude * axisVector.magnitude) * 2f) - 1f) /* * Random.Range(-0.2f, 1f)*/ * Time.deltaTime, setScale*0.75f, setScale * 1.25f);


        if (Input.GetButtonUp("B") || Input.GetButtonUp("A")) 
		{
			first = true;

		}
	}

    
    void Draw(Vector2 OldUV, Vector2 NewUV)
    {
		Debug.Log ("old: " + OldUV.ToString () + "new: " + NewUV.ToString ());
		if (!first) 
		{
			Vector2[] positions = FindStampPositions (OldUV, NewUV);
			//Debug.Log (positions.Length.ToString ());
			for (int i = 0; i < positions.Length; ++i) 
			{
				Vector2 p = positions [i];
				Stamp ((int)p.x - scaledBrushW / 2, (int)p.y - scaledBrushH / 2);
				//if (p.x >= 0 && p.x + scaledBrushW <= texture.width && p.y >= 0 && p.y + scaledBrushH <= texture.height)

			}
		} 
		else 
		{
			Stamp ((int)NewUV.x - scaledBrushW / 2, (int)NewUV.y - scaledBrushH / 2);
		}
        texture.Apply(true);
    }

	void Erase(Vector2 OldUV, Vector2 NewUV)
	{
		//Debug.Log ("old: " + OldUV.ToString () + "new: " + NewUV.ToString ());
		if (!first) {
			Vector2[] positions = FindStampPositions (OldUV, NewUV);
			//Debug.Log (positions.Length.ToString ());
			for (int i = 0; i < positions.Length; ++i) 
			{
				Vector2 p = positions [i];
				if (p.x >= 0 && p.x + scaledBrushW <= texture.width && p.y >= 0 && p.y + scaledBrushH <= texture.height)
					EraseStamp((int)p.x - scaledBrushW / 2, (int)p.y - scaledBrushH / 2);
			}
		} 
		else 
		{
			EraseStamp((int)NewUV.x - scaledBrushW / 2, (int)NewUV.y - scaledBrushH / 2);
		}
		texture.Apply(true);
	}
	
	
	Vector2[] FindStampPositions(Vector2 v1, Vector2 v2)
	{
		
		Vector2 dir = (v2 - v1);
		Vector2[] tmp = new Vector2[(int)(Mathf.Max(dir.magnitude / paintinterval,1f))];
		for (int i = 0; i < tmp.Length; ++i) 
		{
			//interpolate the points between the first and last position by moving the position paintinterval px to the next position
			tmp[i] = v1 + i * dir.normalized * paintinterval;
		}
		
		return tmp;
	}
	
	
	void Stamp(int x, int y)
	{
		//Debug.Log ("stamp" + x + "|" + y);
		if (scaleFactor * brush.width != scaledBrushW || first) 
		{
			brushRescale(scaleFactor);
		}

		UpdateScaledBrush(x, y);
		texture.SetPixels(x, y, scaledBrushW, scaledBrushH, currentPx);
	}

	void EraseStamp(int x, int y)
	{
		//Debug.Log ("stamp" + x + "|" + y);
		if (scaleFactor * brush.width != scaledBrushW || first) 
		{
			brushRescale(scaleFactor);
		}
		
		UpdateScaledEraser(x, y);
		texture.SetPixels(x, y, scaledBrushW, scaledBrushH, currentPx);
	}


	void UpdateScaledBrush(int x, int y)
	{
		for (int i = 0; i < scaledBrushW; i++) 
		{
			for (int j = 0; j < scaledBrushH; j++)
			{
				Color c = scaledBrushPx[i + scaledBrushW * j];
				currentPx[scaledBrushW * j + i] = texture.GetPixel(x + i, y + j) + c * c.a;//Color.Lerp(texture.GetPixel(x + i, y + j), c, c.a);
			}
		}
	}

	void UpdateScaledEraser(int x, int y)
	{
		for (int i = 0; i < scaledBrushW; i++) 
		{
			for (int j = 0; j < scaledBrushH; j++)
			{
				Color c = scaledBrushPx[i + scaledBrushW * j];
				if(c.a >= 0.3)
					currentPx[scaledBrushW * j + i] = clearColor;
				else
				{
					currentPx[scaledBrushW * j + i] = texture.GetPixel(x + i, y + j);
				}
				//Debug.Log (currentPx[scaledBrushW * j + i].ToString());
			}
		}
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
		Gizmos.DrawRay(brushCircle.transform.position, speed);
        
		Gizmos.color = Color.red;
		if(maxCorner != null)
			Gizmos.DrawLine (transform.position, maxCorner.position);
		Gizmos.color = Color.blue;
		if(minCorner != null)
			Gizmos.DrawLine (transform.position, minCorner.position);

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

	void floodTexture(Color color)
	{
		Color[] c = new Color[texture.width * texture.height];
		for(int i = 0; i < c.Length; i++)
		{
			c[i] = color;
		}

		texture.SetPixels (c);
	}

}
