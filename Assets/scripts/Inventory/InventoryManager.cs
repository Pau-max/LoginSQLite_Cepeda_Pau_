using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour   
{
    public InventoryRepository repo;
    public Transform panelContainer; 
    public GameObject itemSlotPrefab;

    [Header("Usuario Actual")]
    public int currentUserId; 

    void Start() 
    { 
        if(currentUserId != 0) RefreshUI(); 
    }

    public void AddItem(int id)
    {
        repo.SaveOrUpdateItem(currentUserId, id, 1);
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in panelContainer) Destroy(child.gameObject);

        List<InventoryEntry> items = repo.GetInventory(currentUserId);

        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, panelContainer);

            
            slot.GetComponentInChildren<Text>().text = item.itemName; 
            
            var qtyText = slot.transform.Find("QtyText")?.GetComponent<Text>();
            if(qtyText != null) qtyText.text = "x" + item.itemQuantity;

            
            Button delBtn = slot.transform.Find("DeleteBtn")?.GetComponent<Button>();
            if(delBtn != null)
            {
                int idObj = item.itemID;
                delBtn.onClick.AddListener(() => {
                    repo.RemoveOneItem(currentUserId, idObj);
                    RefreshUI();
                });
            }
        }
    }
}