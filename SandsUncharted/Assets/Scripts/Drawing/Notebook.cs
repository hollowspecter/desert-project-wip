using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notebook : MonoBehaviour
{
    //[HideInInspector]
    public GameObject pagePrefab;

    List<GameObject> pages;

    GameObject currPage;

    GameObject turningPage;
    bool turningNext = false;
    float turningSpeed = 120f;

	// Use this for initialization
	void Start ()
    {
        if(pagePrefab != null)
        {
            pages = new List<GameObject>();
            NewPage();
        }
        else
        {
            Debug.LogError("There is no page prefab!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(turningNext)
        {
            if(turningPage.transform.localRotation.eulerAngles.z < 180f)
            {
                Debug.Log(turningPage.transform.localRotation.eulerAngles.z);
                TurnPage(true);
            }
            else
            {
                turningNext = false;
                ActivatePageDraw(currPage);
            }
        }

        if (Input.GetButtonDown("RB"))
        {
            ChangePage(true);
        }

	}


    //Adds a page to the end of the book and makes it current
    void AddPage(GameObject g)
    {
        pages.Add(g);
        currPage = g;
    }

    //Removes a page from the notebook and makes the next page current
    void RemovePage(GameObject g)
    {
        currPage = GetNextPage(g);
        pages.Remove(g);
    }

    // get the page before g
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

    //get the page after g
    GameObject GetNextPage(GameObject g)
    {
        int currIndex = pages.IndexOf(g);
        int index = (int)Mathf.Min(currIndex + 1, pages.Count - 1);
        GameObject tmp = pages[index];
        if (tmp != null)
            return tmp;
        else
        {
            Debug.LogError("no page found");
            return null;
        }
    }

    void ActivatePage(GameObject g)
    {
        g.SetActive(true);
    }

    void DeactivatePageDraw(GameObject g)
    {
        GameObject page = g.transform.Find("page").gameObject;
        page.GetComponent<DrawingScript>().enabled = false;
    }
    void ActivatePageDraw(GameObject g)
    {
        GameObject page = g.transform.Find("page").gameObject;
        page.GetComponent<DrawingScript>().enabled = true;
    }

    //Turn the Page in either direction, forward means going to the next page
    void ChangePage(bool forward)
    {
        int currIndex = pages.IndexOf(currPage);
        if(forward)
        {
            Debug.Log("we are at page:" + currIndex + " of:" + (pages.Count - 1) + " wanting next page");
            //if we are on the last page, make a new page
            if(currIndex >= pages.Count - 1)
            {
                turningPage = pages[currIndex];
                DeactivatePageDraw(turningPage);
                NewPage();
                DeactivatePageDraw(currPage);
            }
            else
            {
                turningPage = pages[currIndex];
                DeactivatePageDraw(turningPage);
                currPage = GetNextPage(currPage);
                DeactivatePageDraw(currPage);
            }

            turningNext = true;
        }

        if (!forward)
        {
            Debug.Log("we are at page:" + currIndex + " of:" + (pages.Count - 1) + " wanting last page");
            //if we are on the last page, make a new page
            if (currIndex <= 0)
            {
                turningPage = pages[currIndex];
                DeactivatePageDraw(turningPage);
                NewPage();
                DeactivatePageDraw(currPage);
            }
            else
            {
                turningPage = pages[currIndex];
                DeactivatePageDraw(turningPage);
                currPage = GetNextPage(currPage);
                DeactivatePageDraw(currPage);
            }

            turningNext = true; 
        }
    }

    void TurnPage(bool next)
    {
        float direction = next ? 1f : -1f;
        float angle = direction * turningSpeed * Time.deltaTime;
        turningPage.transform.Rotate(Vector3.forward * angle, Space.Self);
    }

    void NewPage()
    {
        GameObject g = GameObject.Instantiate(pagePrefab);
        g.transform.SetParent(transform);
        g.transform.localPosition = new Vector3();
        g.transform.localRotation = new Quaternion();
        g.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        AddPage(g);
    }
}
