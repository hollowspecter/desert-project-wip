using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
    private static int id = 0;
    private string itemName;

    public string ItemName { get { return itemName; } }

    void Start()
    {
        itemName = "ID: " + ++id;
    }

    public void Selected()
    {
        Debug.Log("Selected " + itemName);
    }
}
