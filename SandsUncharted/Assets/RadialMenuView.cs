using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RadialMenuView : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabMenuItems;
    [SerializeField]
    private float radiusReduction = 10f;
    [SerializeField]
    private float radiusArrowRatio = 0.7f;
    [SerializeField]
    private float radiusItemRadio = 0.9f;

    private float radius;
    private float angle;
    private RadialMenu menu;
    private List<RadialMenuItem> items;
    private int lastSelected;

    //RectTransforms
    private RectTransform wheel;
    private RectTransform arrow;
    private RectTransform arrowSprite;
    private RectTransform itemsTransform;

    void Awake()
    {
        lastSelected = -1;

        items = new List<RadialMenuItem>();
        
        menu = RadialMenu.Instance();

        radius = ((float)Screen.height / 2f) - radiusReduction;

        wheel = transform.FindChild("Wheel").GetComponent<RectTransform>();
        Assert.IsNotNull<RectTransform>(wheel);
        arrow = transform.FindChild("Arrow").GetComponent<RectTransform>();
        Assert.IsNotNull<RectTransform>(wheel);
        arrowSprite = arrow.GetChild(0).GetComponent<RectTransform>();
        Assert.IsNotNull<RectTransform>(arrowSprite);
        itemsTransform = transform.FindChild("items").GetComponent<RectTransform>();
        Assert.IsNotNull<RectTransform>(wheel);

        if (prefabMenuItems.Length < 0)
            Debug.LogError("No Menu Item Prefabs!");

        InitialiseWheel();
    }

    void Start()
    {
        CreateMenuItems();
        ToggleActivation(false);
    }

    void ToggleActivation(bool b)
    {
        wheel.gameObject.SetActive(b);
        arrow.gameObject.SetActive(b);
        itemsTransform.gameObject.SetActive(b);
    }

    void Activate() { ToggleActivation(true); Debug.Log("Menu Activated"); }

    /// <summary>
    /// When Deactivated, the Action of the Menu Item is called!
    /// </summary>
    void Deactivate() {
        ToggleActivation(false);
        Debug.Log("Menu Deactivated");
        items[lastSelected].Action();
    }

    void InitialiseWheel()
    {
        // Set wheel
        wheel.sizeDelta = new Vector2(radius*2, radius*2);

        // Set ArrowSprite
        float x = arrowSprite.sizeDelta.x;
        float y = radius * radiusArrowRatio;
        arrowSprite.sizeDelta = new Vector2(x, y);
    }

    /// <summary>
    /// Instantiates the number of menu items from the prefabMenuItems Array
    /// </summary>
    void CreateMenuItems()
    {
        int number = menu.NumberOfItems;

        for (int i = 0; i < number; ++i) {
            //Retrieve Inventory Item
            InventoryItem item = menu.getItem(i);

            // Instantiate GO
            GameObject g;
            if (prefabMenuItems.Length <= number)
                g = Instantiate(prefabMenuItems[i]);
            else {
                g = Instantiate(prefabMenuItems[0]);
                Debug.LogWarning("The number of items were higher than the number of item prefabs in the inspector!", this);
            }
            RectTransform _transform = g.GetComponent<RectTransform>();

            // Retrieve Item Script
            RadialMenuItem menuItemScript = g.GetComponent<RadialMenuItem>();
            items.Add(menuItemScript);

            // Parent it under Items
            _transform.SetParent(itemsTransform, false);

            // Calculate Angle
            float angle = Mathf.Lerp(0f, 360f, ((float)i) / ((float)number));
            Vector2 position = new Vector2(0, radius * radiusItemRadio);

            // Construct a quaternion shift and multiply to get your final position
            Quaternion shift = Quaternion.Euler(new Vector3(0, 0, -angle));
            position = shift * position;
            _transform.Translate(position);

            // Set the text
            Text t = g.GetComponentInChildren<Text>();
            if (t != null)
                t.text = "MenuItem " + i.ToString();
        }
    }

    void Update()
    {
        // Rotate the arrow
        Vector2 leftStick = menu.LeftStick;
        if (leftStick != Vector2.zero) {
            angle = Vector2.Angle(Vector2.up, leftStick);
            angle *= (leftStick.x > 0f) ? -1f : 1f;
        }
        // Activate this code if you want the arrow to "snap back" when no input
        //else
        //    angle = 0;

        // Select an item
        int selected = GetSelected(menu.NumberOfItems, angle);
        if (lastSelected != selected) {
            if (lastSelected >= 0)
                items[lastSelected].Toggle(false);
            items[selected].Toggle(true);
            lastSelected = selected;
        }

        Vector3 eulerAngles = new Vector3(0, 0, angle);
        arrow.localRotation = Quaternion.Euler(eulerAngles);
    }

    int GetSelected(int numberOfItems, float angle)
    {
        // First, map the angles on 0 to 360 degree
        float tmpAngle = this.angle;
        if (tmpAngle < 0) tmpAngle *= -1f;
        else if (tmpAngle > 0) {
            float newAngle = 180;
            newAngle += 180f - (tmpAngle % 360f);
            tmpAngle = newAngle;
        }

        float step = 360f/((float)numberOfItems);
        tmpAngle = (tmpAngle /*- (step / 2f)*/) % 360f;
        return Mathf.RoundToInt(tmpAngle / step) % numberOfItems;
    }

    void OnEnable()
    {
        InventoryState.OnEnter += Activate;
        InventoryState.OnExit += Deactivate;
    }

    void OnDisable()
    {
        InventoryState.OnEnter -= Activate;
        InventoryState.OnExit -= Deactivate;
    }
}
