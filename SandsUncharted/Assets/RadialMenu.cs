using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadialMenu : MonoBehaviour
{
    #region singleton
    // singleton
    private static RadialMenu radialMenu;
    public static RadialMenu Instance()
    {
        if (!radialMenu) {
            radialMenu = FindObjectOfType(typeof(RadialMenu)) as RadialMenu;
            if (!radialMenu)
                Debug.LogError("There needs to be one active GameManager on a GameObject in your scene.");
        }

        return radialMenu;
    }
    #endregion

    #region private
    [SerializeField]
    private int numberOfItems;
    [SerializeField]
    private GameObject itemPrefab;
    
    private Transform itemsParent;
    private float leftX = 0;
    private float leftY = 0;

    #endregion

    #region public

    public int NumberOfItems { get { return numberOfItems; } }
    public Vector2 LeftStick { get { return new Vector2(leftX, leftY); } }

    #endregion

    void Awake()
    {
        itemsParent = transform.FindChild("Items").transform;

        for (int i = 0; i < numberOfItems; ++i) {
            Debug.Log("Item created");
            GameObject g = Instantiate(itemPrefab);
            g.transform.parent = itemsParent;
        }
    }

    void Update()
    {
        leftX = Input.GetAxis("Horizontal");
        leftY = Input.GetAxis("Vertical");
    }
}
