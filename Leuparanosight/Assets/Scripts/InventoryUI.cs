using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Inventory inventory;
    public Transform slotParent;
    public GameObject slotPrefab;

    private bool isOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);

        for (int i = 0; i < inventory.slotCount; i++)
        {
            Instantiate(slotPrefab, slotParent);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);

            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;
        }

        for (int i = 0; i < slotParent.childCount; i++)
        {
            var slotData = inventory.slots[i];
            var slotUI = slotParent.GetChild(i);

            Image icon = slotUI.Find("Icon").GetComponent<Image>();
            Text amount = slotUI.Find("Amount").GetComponent<Text>();

            if (!string.IsNullOrEmpty(slotData.itemName))
            {
                icon.sprite = slotData.icon;
                icon.enabled = true;
                amount.text = slotData.amount.ToString();
            }
            else
            {
                icon.enabled = false;
                amount.text = "";
            }
        }
    }
}
