using UnityEngine;

public class AddItem : MonoBehaviour
{
    // Ya no dependemos de una variable pública arrastrada manualmente
    
    public void AñadirTostada()
    {
        // Buscamos la instancia que realmente tiene el ID del login
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(1); 
        }
    }

    public void AñadirEspada()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(2);
        }
    }
}