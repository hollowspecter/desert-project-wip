using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class DrawingScript : MonoBehaviour 
{
	public Texture2D brush;
	public GameObject brushCircle;

    Texture2D texture;
    int texSize = 512;

    Color clearColor;
    Camera cam;

	bool first;
	Vector2 oldUV;
	Vector2 pixelUV;
    
    float reticleAcceleration = 1.6f;
	Vector3 speed = new Vector3();

	float paintinterval = 1f; // how many pixels between two drawpositions

	Color[] brushPx;
	Color[] currentPx;
	Color[] scaledBrushPx;

	int scaledBrushW;
	int scaledBrushH;
	float scaleFactor = 0.5f;
	float setScale = 0.5f;
    float eraseScale = 1f;
    float brushScale = 0.5f;
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


    Texture2D[] backups;
    int maxBackups = 5;
    int numBackups = 0;
    int currBackup  = 0;

    public GameObject uiPanel;

    void Awake() 
	{
        maxCorner = transform.parent.FindChild("rightEdge");
        minCorner = transform.parent.FindChild("topEdge");

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        float ratioWH = (maxCorner.position - minCorner.position).x / (maxCorner.position - minCorner.position).z;
		texture = new Texture2D((int)(texSize * ratioWH), texSize, TextureFormat.ARGB32, false);
		clearColor  = new Color(0f,0f,0f,0f);
		FloodTexture (clearColor, texture);
		mesh = GetComponent<MeshRenderer> ();
		if (mesh == null) 
		{
			Debug.LogError("no mesh");
		}
		mesh.material.SetTexture("_DrawTex", texture);

		meshWidth = mesh.bounds.size.x;
		meshHeight = mesh.bounds.size.z;

		brushPx = brush.GetPixels ();
		oldUV = pixelUV = new Vector2 ();
		first = true;
		brushCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		brushRescale (scaleFactor, brush);

        backups = new Texture2D[maxBackups];
        for(int i = 0; i < maxBackups; ++i)
        {
            backups[i] = new Texture2D((int)(texSize * ratioWH), texSize, TextureFormat.ARGB32, false);
            FloodTexture(clearColor, backups[i]);
        }

        NewBackup();
	}

	void Update() 
	{
        uiPanel.SetActive(gameObject.active);

        RaycastHit mouseHit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast (ray, out mouseHit))
        {
            //brushCircle.transform.position = mouseHit.point + transform.up * 0.01f;
        }

        //Debug.DrawLine(ray.origin, mouseHit.point, Color.red);

        /****move Reticle with joystick****/
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 axisVector = h * transform.right + v * transform.forward;
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();
        //Debug.DrawRay (brushCircle.transform.position, axisVector);
		speed += reticleAcceleration * axisVector * Time.deltaTime;

		brushCircle.transform.position += speed * Time.deltaTime;

		speed *= 0.75f;//friction

		//brushCircle.transform.position += (reticleSpeed * reticleSpeed) * axisVector * Time.deltaTime; //squared speed to get a better controllable curve

		if (mesh == null) 
		{
			Debug.LogError("no mesh in update");
		}
		//clamp brushposition to mapborder
		brushCircle.transform.localPosition = new Vector3 (Mathf.Clamp (brushCircle.transform.localPosition.x, minCorner.localPosition.x + border, maxCorner.localPosition.x - border), brushCircle.transform.localPosition.y, Mathf.Clamp(brushCircle.transform.localPosition.z, minCorner.localPosition.z + border, maxCorner.localPosition.z - border));

        /* free brush scaling */

        float scaleAxis = Input.GetAxis("RightStickX");
        if (Mathf.Abs(scaleAxis) > 0.2)
        {
            if (scaleAxis < 0)
            {
                scaleFactor = Mathf.Clamp(scaleFactor + (-1.0f * scaleSpeed * Time.deltaTime), minPX / brush.width, maxPX / brush.width);
                setScale = scaleFactor;
            }
            else
            {
                scaleFactor = Mathf.Clamp(scaleFactor + (1.0f * scaleSpeed * Time.deltaTime), minPX / brush.width, maxPX / brush.width);
                setScale = scaleFactor;
            }

            brushCircle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        }

        //rescale based on speed slow = big / fast = small
        scaleFactor = Mathf.Clamp(scaleFactor + (((1f - axisVector.magnitude * axisVector.magnitude) * 2f) - 1f) /* * Random.Range(-0.2f, 1f)*/ * Time.deltaTime, setScale * 0.75f, setScale * 1.25f);


        /* draw on buttonpressfrom brushreticle*/
        if (Input.GetButton("A"))
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
                    setScale = brushScale;
					brushRescale (scaleFactor, brush);
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
                    setScale = eraseScale;
					brushRescale (scaleFactor, brush);
				}
				
				Erase(oldUV, pixelUV);
			}
		}

        else if(Input.GetButtonUp("Y"))
        {
            SaveTexture();
        }

        //clear the texture
        else if(Input.GetButtonUp("X"))
        {
            FloodTexture(clearColor, texture);
            NewBackup();
        }

        else if(Input.GetButtonDown("LB"))
        {
            GetBackup(true);
        }

        else if (Input.GetButtonDown("RB"))
        {
            GetBackup(false);
        }

        if (Input.GetButtonUp("B") || Input.GetButtonUp("A")) 
		{
			first = true;
            NewBackup();
		}
	}

    
    void Draw(Vector2 OldUV, Vector2 NewUV)
    {
		//Debug.Log ("old: " + OldUV.ToString () + "new: " + NewUV.ToString ());
		if (!first) 
		{
            Debug.Log("notfirststamp");
			Vector2[] positions = FindStampPositions (OldUV, NewUV);
			for (int i = 0; i < positions.Length; ++i) 
			{
				Vector2 p = positions [i];
				Stamp ((int)p.x - scaledBrushW / 2, (int)p.y - scaledBrushH / 2);
			}
		} 
		else 
		{
            Debug.Log("firststamp");
			Stamp ((int)NewUV.x - scaledBrushW / 2, (int)NewUV.y - scaledBrushH / 2);
            first = false;
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
            first = false;
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
		Debug.Log ("stamp" + x + "|" + y);
	    brushRescale(scaleFactor, brush);

		UpdateScaledBrush(x, y);
		texture.SetPixels(x, y, scaledBrushW, scaledBrushH, currentPx);
	}

	void EraseStamp(int x, int y)
	{
		//Debug.Log ("stamp" + x + "|" + y);
	    brushRescale(scaleFactor, brush);
		
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
        /*
        Gizmos.color = Color.green;
		Gizmos.DrawRay(brushCircle.transform.position, speed);
        
		Gizmos.color = Color.red;
		if(maxCorner != null)
			Gizmos.DrawLine (transform.position, maxCorner.position);
		Gizmos.color = Color.blue;
		if(minCorner != null)
			Gizmos.DrawLine (transform.position, minCorner.position);
        */
    }

	void brushRescale(float factor, Texture2D b)
	{
		scaledBrushW = (int) (b.width * factor);
		scaledBrushH = (int) (b.height * factor);

		scaledBrushPx = new Color[scaledBrushW * scaledBrushH];
		currentPx = new Color[scaledBrushW * scaledBrushH];
		//Nearest Neighbor testing
		for (int x = 0; x < scaledBrushW; x++) 
		{
			for(int y = 0; y < scaledBrushW; y++)
			{
				//Debug.Log ("newX/Y: " + x + "/" + y + " oldX/Y: " + Mathf.RoundToInt(x/factor) + "/" + Mathf.RoundToInt(y/factor));
				scaledBrushPx[x + scaledBrushW * y] = b.GetPixel(Mathf.RoundToInt(x/factor), Mathf.RoundToInt(y/factor));
			}
		}
	}

	void FloodTexture(Color color, Texture2D tex)
	{
		Color[] c = new Color[tex.width * tex.height];
		for(int i = 0; i < c.Length; i++)
		{
			c[i] = color;
		}

		tex.SetPixels (c);
        tex.Apply();
	}

    void SaveTexture()
    {
        Debug.Log("save");
        Texture2D tex;
        tex = texture;


        // encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        // set path variables
        string tPath = "/Resources/Map/";

        // create the default path for textures
        System.IO.Directory.CreateDirectory(Application.dataPath + tPath);

        // save the encoded texture file
        File.WriteAllBytes(Application.dataPath + tPath + "map" + ".png", bytes);
    }

    void LoadTexture()
    {
        Debug.Log("load");
    }

    void CopyTexture(Texture2D source, Texture2D target)
    {
        if(source.width == target.width && source.height == target.height)
        {
            target.SetPixels(source.GetPixels());
            target.Apply();
        }    
    }


    //BACKUP METHODS for undoing
    void NewBackup()
    {
        if (currBackup != 0)
        {
            //Shift the current backup to slot 1
            ResetBackupPositions();
            //Copy the texture into the first slot
            CopyTexture(texture, backups[0]);
            //set the backup to -1 to indicate that there was a new backup saved
            currBackup = 0;
        }

        else
        {
            Debug.Log("Backup");
            //Move the Backups over one slot
            ShiftBackups();
            //Copy the texture into the first slot
            CopyTexture(texture, backups[0]);
            //set the backup to -1 to indicate that there was a new backup saved
            currBackup = 0;
        }

        numBackups = (int)Mathf.Min((numBackups + 1), maxBackups-1);
    }

    void GetBackup(bool undo)
    {
        //if we are in slot 0 we cannot redo
        if(currBackup == 0 && !undo)
        {
            Debug.Log("No newer versions");
        }
        //if we are in the last slot we cannot undo
        else if(currBackup == numBackups && undo)
        {
            Debug.Log("no older version");
        }
        //else, we just switch over to the next or last backup by changing currbackup
        else
        {
            if(undo)
            {
               currBackup = (int)Mathf.Min(currBackup + 1, numBackups);
            }

            else
            {
                currBackup = (int)Mathf.Max(currBackup - 1, 0f);
            }
            Debug.Log((undo ? "undo " : "redo ") + currBackup);
            CopyTexture(backups[currBackup], texture);
        }
    }

    //Move all backups over one slot 0->1, 1->2 etc. last backup is thrown away
    void ShiftBackups()
    {
        for(int i = maxBackups-2; i >= 0; --i)
        {
            CopyTexture(backups[i], backups[i + 1]);
            //Debug.Log(i + " -> " + (i +1));
        }
    }

    //Move your current backup to slot 1 because you changed in while of undoing
    void ResetBackupPositions()
    {
        int offset = (int) Mathf.Max(currBackup - 1, 0);
        for(int i = currBackup; i <= numBackups; ++i)
        {
            CopyTexture(backups[i], backups[i - offset]);
            FloodTexture(clearColor, backups[i]);
            //Debug.Log(i + " -> " + (i - offset));
        }

        numBackups = maxBackups - currBackup;
    }

}