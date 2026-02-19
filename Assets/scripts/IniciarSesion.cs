using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using System.IO;

public class IniciarSesion : MonoBehaviour
{
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContraseña;
    public UnityEngine.UI.Button botonLogin;
    public TextMeshProUGUI mensaje;

    public GameObject canvasLogin;   
    public GameObject canvasPrincipal; 

    void Start()
    {
        botonLogin.onClick.AddListener(ComprobarLogin);
    }

    void ComprobarLogin()
    {
        string rutaDB = Path.Combine(Application.dataPath, "Plugins", "UserAndInventory.sqlite");
        string dbUri = "URI=file:" + rutaDB;

        using (var conexion = new SqliteConnection(dbUri))
        {
            conexion.Open();
            // Obtenemos el ID único del usuario
            string consulta = "SELECT id FROM Usuarios WHERE usuario=@usuario AND password=@password LIMIT 1";

            using (var comando = new SqliteCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", inputUsuario.text);
                comando.Parameters.AddWithValue("@password", inputContraseña.text);

                object resultado = comando.ExecuteScalar();

                if (resultado != null)
                {
                    int idDetectado = System.Convert.ToInt32(resultado);
                    Debug.Log("<color=green>Login Correcto.</color> ID Detectado: " + idDetectado);
                    
                    // LLAMADA CLAVE: Enviamos el ID al Manager ANTES de entrar
                    EntrarAlJuego(idDetectado);
                }
                else
                {
                    mensaje.text = "Usuario o contraseña incorrectos";
                }
            }
        }
    }

    void EntrarAlJuego(int id)
    {
        // 1. Intentamos buscar la instancia por Singleton
        InventoryManager manager = InventoryManager.Instance;

        // 2. Si es nula, la buscamos manualmente en la escena por si acaso
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<InventoryManager>();
        }

        if (manager != null)
        {
            manager.currentUserId = id; 
            manager.RefreshUI(); 
            Debug.Log("<color=cyan>ID enviado con éxito al Manager: </color>" + id);
        }
        else 
        {
            // Si sale este error, es que el objeto "InventoryManager" NO ESTÁ en tu jerarquía de Unity
            Debug.LogError("ERROR CRÍTICO: El objeto InventoryManager no existe en esta escena.");
            return; // No entramos al juego si no hay manager
        }

        canvasPrincipal.SetActive(true);
        canvasLogin.SetActive(false);
    }   
}