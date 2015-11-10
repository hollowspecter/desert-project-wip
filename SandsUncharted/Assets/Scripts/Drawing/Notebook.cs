using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notebook : MonoBehaviour
{
    //[HideInInspector]
    public GameObject pagePrefab;

    private Animator _anim; // currently turning pages animator

    List<GameObject> pages;

    GameObject currPage;

    GameObject turningPage;

    int turning = 0;
    float turningSpeed = 120f;
    
    void Awake()
    {
    }

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
        //if turndirection is not 0 we want to turn the page
        if(turning != 0)
        {
            AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
            AnimatorTransitionInfo transitionInfo = _anim.GetAnimatorTransitionInfo(0);
            if ((transitionInfo.IsName("Base Layer.PageFlipNext -> Base Layer.IdleLeft") || transitionInfo.IsName("Base Layer.PageFlipLast -> Base Layer.IdleRight")))
            {
                //TurnPage();
                turning = 0;
                turningPage = null;
                _anim = null;
                ActivatePageDraw(currPage);
            }
        }

        if (Input.GetButtonDown("RB"))
        {
            ChangePage(true);
        }
        if(Input.GetButtonDown("LB"))
        {
            ChangePage(false);
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
        if (turningPage == null)
        {
            int currIndex = pages.IndexOf(currPage);
            //Do we want to go to the next or the last page
            if (forward)
            {
                Debug.Log("we are at page:" + currIndex + " of:" + (pages.Count - 1) + " wanting next page");
                //if we are on the last page, make a new page
                if (currIndex >= pages.Count - 1)
                {
                    turningPage = pages[currIndex];
                    DeactivatePageDraw(turningPage);
                    NewPage();
                    DeactivatePageDraw(currPage);
                    turning = 1;
                }
                else
                {
                    turningPage = pages[currIndex];
                    DeactivatePageDraw(turningPage);
                    currPage = GetNextPage(currPage);
                    DeactivatePageDraw(currPage);
                    turning = 1;
                }
                _anim = turningPage.GetComponent<Animator>();
                _anim.SetTrigger("NextPage");
            }

            if (!forward)
            {
                Debug.Log("we are at page:" + currIndex + " of:" + (pages.Count - 1) + " wanting last page");
                //if we are not on the first page go to the page before this one
                if(currIndex > 0f)
                {
                    turningPage = pages[currIndex - 1];
                    DeactivatePageDraw(turningPage);
                    currPage = GetBeforePage(currPage);
                    DeactivatePageDraw(currPage);
                    turning = -1;
                    _anim = turningPage.GetComponent<Animator>();
                    _anim.SetTrigger("LastPage");
                }
                
            }
        }
    }

    void TurnPage()
    {
        Debug.Log("turn");
        float angle = turning * 180f;
        turningPage.transform.Rotate(Vector3.forward * angle, Space.Self);
        Debug.Log("turningPageRotationZ: " + turningPage.transform.rotation.eulerAngles.z);
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
