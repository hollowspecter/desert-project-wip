using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] images;
    private int selectedIndex = 0;

    [SerializeField]
    private GameObject _stampPrefab;
    private List<GameObject> objects;
    private int maxObjects = 10;
    [SerializeField]
    private Transform _parentTransform;

	// Use this for initialization
	void Start ()
    {
        objects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void ChooseImage(int i)
    {
        selectedIndex = i;
    }

    public Sprite GetSelected()
    {
        return images[selectedIndex];
    }

    public bool IsFull()
    {
        return objects.Count >= maxObjects;
    }

    public void StampSelectedImage(Vector3 position, Quaternion rotation, float scale)
    {
        if(objects.Count >= maxObjects)
        {
            RemoveFirstStamp();
        }
        AddStamp(position, rotation, scale);
    }

    void AddStamp(Vector3 position, Quaternion rotation, float scale)
    {
        GameObject g = (GameObject)GameObject.Instantiate(_stampPrefab, position, rotation);
        g.GetComponent<SpriteRenderer>().sprite = images[selectedIndex];
        g.transform.SetParent(_parentTransform, true);
        g.transform.localScale = new Vector3(scale, scale, scale);
        objects.Add(g);
    }

    void RemoveFirstStamp()
    {
        GameObject g = objects[0];
        objects.RemoveAt(0);
        GameObject.DestroyImmediate(g);
    }
}
