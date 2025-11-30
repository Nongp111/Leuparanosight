using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slotCount = 8;  // จำนวนช่องในกระเป๋า
    public List<InventorySlot> slots = new List<InventorySlot>();

    void Start()
    {
        // เติมช่องว่างเริ่มต้น
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(string name, Sprite icon, int amount, ItemType type)
    {
        // ถ้ามี Item แบบเดียวกันแล้ว -> บวกจำนวน
        foreach (var slot in slots)
        {
            if (slot.itemName == name)
            {
                slot.amount += amount;
                return true;
            }
        }

        // ถ้าไม่มี -> หา slot ว่าง
        foreach (var slot in slots)
        {
            if (string.IsNullOrEmpty(slot.itemName))
            {
                slot.itemName = name;
                slot.icon = icon;
                slot.amount = amount;
                slot.itemType = type;
                return true;
            }
        }

        Debug.Log("Inventory Full!");
        return false;
    }

    public void RemoveItem(string name, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.itemName == name)
            {
                slot.amount -= amount;
                if (slot.amount <= 0)
                {
                    // เคลียร์ slot
                    slot.itemName = "";
                    slot.icon = null;
                    slot.amount = 0;
                    slot.itemType = ItemType.Other;
                }
                return;
            }
        }
    }
}
