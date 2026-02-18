using System;

[Serializable]
public class InventoryEntry
{
    public int itemID;
    public string itemName;
    public int itemQuantity;
    public string spriteName;
}

[Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string description;
    public int rarity; 
}