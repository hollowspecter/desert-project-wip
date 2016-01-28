using UnityEngine;
using System.Collections;

public class RtTest : MonoBehaviour
{
    //[SerializeField]
    RenderTexture[] rt;
    int index = 0;

    [SerializeField]
    GameObject captureTarget;
    [SerializeField]
    private Camera _camera;
    RenderTexture active;
    // Use this for initialization
    void Start ()
    {
        rt = new RenderTexture[3];
        for(int i = 0; i <3; ++i)
        {
            rt[i] = new RenderTexture(256, 256, 24);
        }
        active = rt[index];
        _camera.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetButtonDown("A"))
        {
            //initialize camera and texture
            _camera.enabled = true;
            active = null;
            index = (index + 1) % rt.Length;
            active = rt[index];

            Debug.Log("render texture: " + index);
            //activate rendertexture and link camera

            _camera.targetTexture = active;
            if(!active.IsCreated())
            {
                active.Create();
            }
            RenderTexture.active = _camera.targetTexture;
            _camera.Render();
            captureTarget.GetComponent<MeshRenderer>().materials[0].mainTexture = active;
            //_camera.targetTexture = null;
            _camera.enabled = false;
        }
	}
}
