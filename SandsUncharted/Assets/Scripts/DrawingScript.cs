using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawingScript : MonoBehaviour 
{
	public Texture2D brush;
	public GameObject brushCircle;
	
	public GameObject plane;

    public Text xUI;
    public Text yUI;

    Texture2D texture;
    public Texture2D saveTexture;
    Camera cam;

	bool first;
	Vector2 oldUV;
	Vector2 pixelUV;

    float reticleSpeed = 2.0f;

	float paintinterval = 0.1f;

	Color[] brushPx;

	void Start() 
	{
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		texture = new Texture2D(1024,1024, TextureFormat.ARGB32, false);

		plane.GetComponent<MeshRenderer> ().material.SetTexture(0, texture);
		brushPx = brush.GetPixels ();
		Debug.Log (brush.GetPixel(0,0).ToString());
		oldUV = pixelUV = new Vector2 ();
		first = true;
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

        //h = Mathf.Abs(h) > 0.3 ? h : 0;
        //v = Mathf.Abs(v) > 0.3 ? v : 0;

        if (xUI != null && yUI != null)
        {
            xUI.text = "X: " + Input.GetAxis("Horizontal").ToString();
            yUI.text = "Y: " + Input.GetAxis("Vertical").ToString();
        }

        Vector3 axisVector = new Vector3(h, 0, v);
        axisVector = axisVector.sqrMagnitude >= 0.03 ? axisVector : new Vector3();
        brushCircle.transform.position += reticleSpeed * axisVector * Time.deltaTime;

        /* get  drawPosition from brushreticle*/
        if(Input.GetButton("A"))
        {
            RaycastHit reticleHit;
            if(Physics.Raycast(new Ray(brushCircle.transform.position, -transform.up), out reticleHit))
            {
                oldUV = pixelUV;
                pixelUV = reticleHit.textureCoord;

                pixelUV.x *= texture.width;
			    pixelUV.y *= texture.height;
			    if(first)
			    {
				    oldUV = pixelUV;
				    first = false;
			    }

                Draw(oldUV, pixelUV);
            }

        }

        if (Input.GetMouseButtonUp(0) || Input.GetButton("A")) 
		{
			first = true;
		}
	}

    
    void Draw(Vector2 OldUV, Vector2 NewUV)
    {
        Vector2[] positions = FindStampPositions(OldUV, NewUV);
		Debug.Log (positions.Length.ToString ());
        for (int i = 0; i < positions.Length; ++i)
        {
            Vector2 p = positions[i];
            if (p.x >= 0 && p.x + brush.width <= texture.width && p.y >= 0 && p.y + brush.height <= texture.height)
                Stamp((int)p.x - brush.width/2, (int)p.y - brush.height/2);
        }
        texture.Apply(true);
        saveTexture.SetPixels(texture.GetPixels());
        saveTexture.Apply();
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

	void Stamp(int x, int y)
	{
		UpdateBrush(x, y);
		texture.SetPixels(x, y, brush.width, brush.height, brushPx);
	}

	Vector2[] FindStampPositions(Vector2 v1, Vector2 v2)
	{

		Vector2 dir = (v2 - v1);
		Vector2[] tmp = new Vector2[(int)(Mathf.Max(dir.magnitude / 2,1f))];

		for (int i = 0; i < tmp.Length; ++i) 
		{
			tmp[i] = v1 + i * dir.normalized*paintinterval;
		}

		return tmp;
	}

}
