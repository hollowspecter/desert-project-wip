using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadialMenu : MonoBehaviour
{
    #region singleton
    // singleton
    private static RadialMenu rasialMenu;
    public static RadialMenu Instance()
    {
        if (!rasialMenu) {
            rasialMenu = FindObjectOfType(typeof(RadialMenu)) as RadialMenu;
            if (!rasialMenu)
                Debug.LogError("There needs to be one active GameManager on a GameObject in your scene.");
        }

        return rasialMenu;
    }
    #endregion

    #region private
    [SerializeField]
    private int numberOfItems;
    [SerializeField]
    private GameObject itemPrefab;

    private List<InventoryItem> items;
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
        items = new List<InventoryItem>();
        itemsParent = transform.FindChild("Items").transform;

        for (int i = 0; i < numberOfItems; ++i) {
            Debug.Log("Item created");
            GameObject g = Instantiate(itemPrefab);
            g.transform.parent = itemsParent;
            items.Add(g.GetComponent<InventoryItem>());
        }
    }

    void Update()
    {
        leftX = Input.GetAxis("Horizontal");
        leftY = Input.GetAxis("Vertical");
    }

    public InventoryItem getItem(int index)
    {
        if (index < items.Count)
            return items[index];
        else {
            Debug.LogError("Could not return item of index: " + index, this);
            return null;
        }
    }
}
