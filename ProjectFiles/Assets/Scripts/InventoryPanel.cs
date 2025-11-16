using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private SubMenuButton inventorySlotPrefab;
    private List<SubMenuButton> inventorySlots = new List<SubMenuButton>();

    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    [SerializeField] private int displayItemCount = 14;
    private int selectedIndex = 0;
    private int windowStartIndex = 0;

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
        foreach (Transform child in transform.GetChild(0))
            Destroy(child.gameObject);

        inventorySlots.Clear();

        itemSlots = GameManager.instance.items;
        selectedIndex = 0;
        windowStartIndex = 0;

        RedrawWindow();
    }

    private void RedrawWindow()
    {
        foreach (Transform child in transform.GetChild(0))
            Destroy(child.gameObject);

        inventorySlots.Clear();

        if (itemSlots.Count == 0)
        {
            descriptionText.text = "";
            return;
        }

        int windowEnd = Mathf.Min(windowStartIndex + displayItemCount, itemSlots.Count);

        for (int i = windowStartIndex; i < windowEnd; i++)
        {
            SubMenuButton newItem = Instantiate(inventorySlotPrefab, transform.GetChild(0), false);
            newItem.SetSkill(itemSlots[i].item);
            inventorySlots.Add(newItem);

            if (i == selectedIndex)
                newItem.SelectButton();
            else
                newItem.DeselectButton();
        }

        descriptionText.text = itemSlots[selectedIndex].item.description;
    }

    public void SwitchSelected(int increaseValue)
    {
        if (itemSlots.Count == 0)
            return;

        selectedIndex += increaseValue;

        if (selectedIndex < 0)
            selectedIndex = itemSlots.Count - 1;
        else if (selectedIndex >= itemSlots.Count)
            selectedIndex = 0;

        AdjustWindow();
        RedrawWindow();
    }

    private void AdjustWindow()
    {
        if (selectedIndex < windowStartIndex)
        {
            windowStartIndex = selectedIndex;
        }
        else if (selectedIndex >= windowStartIndex + displayItemCount)
        {
            windowStartIndex = selectedIndex - (displayItemCount - 1);
        }
        windowStartIndex = Mathf.Clamp(windowStartIndex, 0, Mathf.Max(0, itemSlots.Count - displayItemCount));
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