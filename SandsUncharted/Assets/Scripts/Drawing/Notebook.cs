using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notebook : MonoBehaviour
{
    //[HideInInspector]
    public GameObject pagePrefab;

    private Animator _anim; // currently turning pages animator

    List<GameObject> pages;
    List<bool> pageIsLefts;

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
            pageIsLefts = new List<bool>();
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
                if(turning > 0)
                {
                    Debug.Log("stop turning left");
                    GameObject g = GetBeforePage(turningPage);
                    GameObject h = GetNextPage(turningPage);
                    if (h != null)
                        h.GetComponent<Animator>().SetBool("IsLeftPage", pageIsLefts[pages.IndexOf(h)]);
                    DeactivatePage(g);                     
                }
                if(turning < 0)
                {
                    Debug.Log("stop turning right");
                    GameObject g = GetNextPage(turningPage);
                    GameObject h = GetBeforePage(turningPage);
                    if (h != null)
                        h.GetComponent<Animator>().SetBool("IsLeftPage", pageIsLefts[pages.IndexOf(h)]);
                    DeactivatePage(g);
                }

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
        pageIsLefts.Add(false);
        currPage = g;
    }

    //Removes a page from the notebook and makes the next page current
    void RemovePage(GameObject g)
    {
        currPage = GetNextPage(g);
        pageIsLefts.RemoveAt(pages.IndexOf(g));
        pages.Remove(g);
    }

    // get the page before g
    GameObject GetBeforePage(GameObject g)
    {
        int currIndex = pages.IndexOf(g);
        int index = (int) Mathf.Max(currIndex - 1, 0f);
        GameObject tmp = pages[index];
        if(tmp != null && index != currIndex)
            return tmp;
        else
        {
            Debug.LogError("no page before this one");
            return null;
        }
    }

    //get the page after g
    GameObject GetNextPage(GameObject g)
    {
        int currIndex = pages.IndexOf(g);
        int index = (int)Mathf.Min(currIndex + 1, pages.Count - 1);
        GameObject tmp = pages[index];
        if (tmp != null && currIndex != index)
            return tmp;
        else
        {
            Debug.LogError("no page after this one");
            return null;
        }
    }

    void ActivatePage(GameObject g)
    {
        if (g != null)
        {
            g.SetActive(true);
            g.GetComponent<Animator>().SetBool("IsLeftPage", pageIsLefts[pages.IndexOf(g)]);
        }
    }

    void DeactivatePage(GameObject g)
    {
        if(g != null)
            g.SetActive(false);
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
                    ActivatePage(currPage);
                    DeactivatePageDraw(currPage);
                    turning = 1;
                }
                _anim = turningPage.GetComponent<Animator>();
                pageIsLefts[pages.IndexOf(turningPage)] = true;
                _anim.SetBool("IsLeftPage", true);
                _anim.SetTrigger("NextPage");

            }

            if (!forward)
            {
                Debug.Log("we are at page:" + currIndex + " of:" + (pages.Count - 1) + " wanting last page");
                //if we are not on the first page go to the page before this one
                if(currIndex > 0f)
                {
                    turningPage = pages[currIndex - 1]; 	   //set the turning page to the page before this one
                    DeactivatePageDraw(turningPage);	       //deactivate the drawing on the turningPage
                    DeactivatePageDraw(currPage);	           //deactivate the drawing on the currentPage
					currPage = turningPage;					   //set the currPage to the turning Page
                    ActivatePage(GetBeforePage(turningPage));  //activate the Page before the current Page
                    turning = -1;
                    _anim = turningPage.GetComponent<Animator>();
                    pageIsLefts[pages.IndexOf(turningPage)] = false;
					_anim.SetBool("IsLeftPage", false);
                    _anim.SetTrigger("LastPage");
                }
                
            }
        }
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
