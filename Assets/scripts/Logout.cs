using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    public void CerrarSesion()
    {
        InventoryManager inv = FindObjectOfType<InventoryManager>();
        if (inv != null)
        {
            inv.currentUserId = -1; // Reseteamos el ID a -1
            inv.RefreshUI(); // Limpia la pantalla visualmente
        }

        PlayerPrefs.DeleteKey("UltimoUsuarioGuardado");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}