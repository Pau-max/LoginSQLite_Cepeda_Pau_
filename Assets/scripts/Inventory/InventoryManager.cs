using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour   
{
    public static InventoryManager Instance; // Acceso global

    public InventoryRepository repo;
    public Transform panelContainer; 
    public GameObject itemSlotPrefab;

    [Header("Usuario Actual (Automático)")]
    public int currentUserId = -1; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Si usas varias escenas, descomenta la siguiente línea:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(int itemID)
    {
        // Si el ID es mayor a 0, significa que alguien inició sesión
        if (currentUserId > 0)
        {
            repo.SaveOrUpdateItem(currentUserId, itemID, 1);
            Debug.Log("Item " + itemID + " añadido al usuario " + currentUserId);
            RefreshUI();
        }
        else
        {
            // Este es el error que te salía; ahora el Login debería evitarlo
            Debug.LogError("Error: Intento de añadir item sin usuario activo (ID actual: " + currentUserId + ")");
        }
    }

    public void RefreshUI()
    {
        if (panelContainer == null) return;
        
        // Limpiar slots viejos
        foreach (Transform child in panelContainer) Destroy(child.gameObject);

        // Si no hay nadie logueado, no cargamos nada
        if (currentUserId <= 0) return;

        // Pedir al repositorio solo los items del usuario actual
        List<InventoryEntry> items = repo.GetInventory(currentUserId);

        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, panelContainer);
            slot.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.itemName;
            slot.transform.Find("QtyText").GetComponent<TextMeshProUGUI>().text = "x" + item.itemQuantity;
            
            Sprite s = Resources.Load<Sprite>(item.spriteName);
            if (s != null) slot.transform.Find("IconImage").GetComponent<Image>().sprite = s;
            
            Button delBtn = slot.transform.Find("DeleteBtn").GetComponent<Button>();
            delBtn.onClick.RemoveAllListeners();
            delBtn.onClick.AddListener(() => {
                repo.RemoveOneItem(currentUserId, item.itemID);
                RefreshUI();
            });
        }
    }
}