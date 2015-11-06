using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notebook : MonoBehaviour
{
    [HideInInspector]
    public GameObject page;

    List<GameObject> pages;

    GameObject currPage;

	// Use this for initialization
	void Start ()
    {
        if(page != null)
        {
            pages = new List<GameObject>();
        }
        else
        {
            Debug.LogError("There is no page prefab!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {

	}


    //Adds a page to the end of the book and sets it as the current page
    void AddPage(GameObject g)
    {
        pages.Add(g);
        currPage = g;
    }
    //Removes a page from the notebook and sets the current Page to the one before
    void RemovePage(GameObject g)
    {
        currPage = GetBeforePage(g);
        pages.Remove(g);
    }

    GameObject GetBeforePage(GameObject g)
    {
        int currIndex = pages.IndexOf(g);
        int index = (int) Mathf.Max(currIndex - 1, 0f);
        GameObject tmp = pages[index];
        if(tmp != null)
            return tmp;
        else
        {
            Debug.LogError("no page found");
            return null;
        }
    }
}
