using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public string itemName;
    public Sprite icon;
    public int amount;
    public ItemType itemType;
}

public enum ItemType
{
    Ammo,
    Health,
    Weapon,
    Other
}