using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour   
{
    public InventoryRepository repo;
    public Transform panelContainer; 
    public GameObject itemSlotPrefab;

    public int currentUserId = 1;

    void Start() { RefreshUI(); }

    public void AddItem(int id)
    {
        bool success = repo.SaveOrUpdateItem(currentUserId, id, 1);
        if (success)
        {
            RefreshUI();
        }
        else
        {
            Debug.Log("Inventario lleno: Máximo 14 objetos diferentes.");
        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in panelContainer) Destroy(child.gameObject);

        List<InventoryEntry> items = repo.GetInventory(currentUserId);

        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, panelContainer);


            slot.transform.Find("NameText").GetComponent<Text>().text = item.itemName;
            slot.transform.Find("QtyText").GetComponent<Text>().text = "x" + item.itemQuantity;

            Sprite icon = Resources.Load<Sprite>(item.spriteName);
            if (icon != null)
            {
                slot.transform.Find("IconImage").GetComponent<Image>().sprite = icon;
            }

            int idParaBorrar = item.itemID;
            slot.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(() => {
                repo.DeleteItem(currentUserId, idParaBorrar);
                RefreshUI();
            });
        }
    }
}