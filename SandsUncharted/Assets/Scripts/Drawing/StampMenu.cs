using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StampMenu : MonoBehaviour
{ 
    [SerializeField]
    private Sprite[] stamps;
    [SerializeField]
    private Transform _menuUI;
    [SerializeField]
    private GameObject _prefab;

    private RectTransform[] items;

    private int stampIndex;
    private bool activated = false;

    private int stampOffset = 130;

    private int columnCount = 3;

    bool moved = false;
    float moveTimer = 0f;

	// Use this for initialization
	void Start ()
    {
        items = new RectTransform[stamps.Length];

        int currentRow = 0;
	    for(int i = 0; i < stamps.Length; ++i)
        {
            if(i != 0 && i % columnCount == 0)
            {
                currentRow++;
            }
            GameObject g = Instantiate(_prefab);
            g.transform.SetParent(_menuUI);
            RectTransform t = g.GetComponent<RectTransform>();
            t.localScale = new Vector3(1, 1, 1);
            t.anchoredPosition = new Vector2(stampOffset/2 + stampOffset * ((i%columnCount)), -(stampOffset/2 + stampOffset * (currentRow)));
            g.transform.GetChild(0).GetComponent<Image>().sprite = stamps[i];
            items[i] = t;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (activated)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float threshhold = 0.3f;

            if (!moved)
            {
                if (h > threshhold)
                {
                    Right();
                }

                if (h < -threshhold)
                {
                    Left();
                }

                if (v > threshhold)
                {
                    Up();
                }
                if (v < -threshhold)
                {
                    Down();
                }
            }
            else
            {
                moveTimer += Time.deltaTime;
                if (moveTimer > 0.25f)
                {
                    moved = false;
                    moveTimer = 0;
                }
            }
            // rescale All Items so that the selected one is bigger
            for (int i = 0; i < stamps.Length; ++i)
            {
                if (i == stampIndex)
                {
                    items[i].localScale = new Vector2(1, 1);
                }
                else
                {
                    items[i].localScale = new Vector2(0.7f, 0.7f);
                }
            }
        }
    }


    public void Activate()
    {
        activated = true;
        _menuUI.gameObject.SetActive(true);
    }

    public Sprite Deactivate()
    {
        activated = false;
        _menuUI.gameObject.SetActive(false);
        return stamps[stampIndex];
    }

    void ClampIndex()
    {
        stampIndex = (int)Mathf.Clamp(stampIndex, 0, stamps.Length - 1);
    }

    public void Right()
    {
        stampIndex++;
        ClampIndex();
        moved = true;
    }

    public void Left()
    {
        stampIndex--;
        ClampIndex();
        moved = true;
    }

    public void Up()
    {
        stampIndex -= columnCount;
        ClampIndex();
        moved = true;
    }

    public void Down()
    {
        stampIndex += columnCount;
        ClampIndex();
        moved = true;
    }
}
