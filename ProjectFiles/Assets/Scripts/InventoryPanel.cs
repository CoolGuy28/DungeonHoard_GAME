using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private SubMenuButton inventorySlotPrefab;
    private List<SubMenuButton> inventorySlots = new List<SubMenuButton>();
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private int displayItemCount;
    private int selectedIndex;
    [SerializeField] private TMP_Text descriptionText;
    private void Start()
    {
        itemSlots = GameManager.instance.items;
    }

    private void OnEnable()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        inventorySlots.Clear();
        foreach (Transform child in transform.GetChild(0))
        {
            Destroy(child.gameObject);
        }
        itemSlots = GameManager.instance.items;
        selectedIndex = 0;
        if (itemSlots.Count > 0)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                SubMenuButton newItem = Instantiate(inventorySlotPrefab, transform.GetChild(0), false);
                newItem.SetSkill(itemSlots[i].item);
                inventorySlots.Add(newItem);
                newItem.DeselectButton();
            }
            inventorySlots[selectedIndex].SelectButton();
            descriptionText.text = inventorySlots[selectedIndex].GetSkill().description;
        }
        else
            descriptionText.text = "";
    }

    public void SwitchSelected(int increaseValue)
    {
        if (itemSlots.Count > 0)
        {
            inventorySlots[selectedIndex].DeselectButton();
            selectedIndex += increaseValue;
            if (selectedIndex < 0)
                selectedIndex = inventorySlots.Count - 1;
            else if (selectedIndex >= inventorySlots.Count)
                selectedIndex = 0;
            inventorySlots[selectedIndex].SelectButton();
            descriptionText.text = inventorySlots[selectedIndex].GetSkill().description;
        }
    }

    public Item GetSelectedItem()
    {
        if (itemSlots.Count > 0)
        {
            return itemSlots[selectedIndex].item;
        }
        return null;
    }
}
