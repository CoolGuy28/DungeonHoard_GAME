using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject inventorySlotPrefab;
    private List<GameObject> inventorySlots = new List<GameObject>();
    [SerializeField] private int padding;
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private void Start()
    {
        itemSlots = GameManager.instance.items;
    }

    public void UpdateInventory()
    {
        inventorySlots.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        itemSlots = GameManager.instance.items;
        int slotY = 437;
        for (int i = 0; i < itemSlots.Count; i++)
        {
            GameObject newItem = Instantiate(inventorySlotPrefab, transform, false);
            if (i % 2 == 0)
            {
                slotY += padding;
                newItem.transform.localPosition = new Vector2(-200, slotY);
            }
            else
            {
                newItem.transform.localPosition = new Vector2(200, slotY);
            }

            inventorySlotPrefab.transform.GetChild(0).GetComponent<TMP_Text>().text = itemSlots[i].item.name;
            inventorySlotPrefab.transform.GetChild(1).GetComponent<TMP_Text>().text = itemSlots[i].quantity.ToString();
            inventorySlots.Add(newItem);
        }
    }
}
