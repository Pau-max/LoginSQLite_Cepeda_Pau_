using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Referencias")]
    public InventoryRepository repo;
    public Transform container; // El 'Content' del ScrollView
    public GameObject rowPrefab; // Prefab con Textos (NameText, QtyText) y Botón (DeleteBtn)

    // Este ID debe ser el que obtuviste en el Login de la Actividad 1
    private int currentUserId = 1;

    void Start()
    {
        RefreshUI();
    }

    // Método para los botones de la UI
    public void OnClickAddItem(int id)
    {
        repo.SaveOrUpdateItem(currentUserId, id, 1);
        RefreshUI();
    }

    public void RefreshUI()
    {
        // 1. Limpiar la lista visual
        foreach (Transform child in container) Destroy(child.gameObject);

        // 2. Cargar datos desde la base de datos
        List<InventoryEntry> items = repo.GetUserInventory(currentUserId);

        // 3. Crear los elementos en la UI
        foreach (var item in items)
        {
            GameObject go = Instantiate(rowPrefab, container);

            // Asignar textos
            go.transform.Find("NameText").GetComponent<Text>().text = item.itemName;
            go.transform.Find("QtyText").GetComponent<Text>().text = "x" + item.itemQuantity;

            // Configurar botón de borrar de cada fila
            int idParaBorrar = item.itemID;
            go.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(() => {
                repo.DeleteItem(currentUserId, idParaBorrar);
                RefreshUI();
            });
        }
    }
}