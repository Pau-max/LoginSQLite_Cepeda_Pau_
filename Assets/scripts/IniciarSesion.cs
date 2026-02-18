using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;

public class IniciarSesion : MonoBehaviour
{
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContraseña;
    public UnityEngine.UI.Button botonLogin;
    public TextMeshProUGUI mensaje;

    public string nombreDB = "MyDatabase.sqlite"; 
    private string rutaDB;

    public GameObject canvasLogin;   
    public GameObject canvasPrincipal; 

    void Start()
    {
        rutaDB = Application.persistentDataPath + "/" + nombreDB;
        botonLogin.onClick.AddListener(ComprobarLogin);
        mensaje.text = "Introduce usuario y contraseña";
    }

    void ComprobarLogin()
    {
        string usuario = inputUsuario.text.Trim();
        string contraseña = inputContraseña.text.Trim();

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
        {
            mensaje.text = "Rellena todos los campos";
            return;
        }

        string dbUri = "URI=file:" + rutaDB;

        using (var conexion = new SqliteConnection(dbUri))
        {
            conexion.Open();
            // Seleccionamos el ID para pasarlo al inventario
            string consulta = "SELECT id FROM Usuarios WHERE usuario=@usuario AND password=@password";

            using (var comando = new SqliteCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@password", contraseña);

                object resultado = comando.ExecuteScalar(); // Obtenemos solo el ID

                if (resultado != null)
                {
                    int idUsuario = System.Convert.ToInt32(resultado);
                    
                    // PASO CRÍTICO: Configurar el InventoryManager con este ID
                    InventoryManager inv = FindObjectOfType<InventoryManager>();
                    if (inv != null)
                    {
                        inv.currentUserId = idUsuario;
                        inv.RefreshUI();
                    }

                    if (canvasPrincipal != null) canvasPrincipal.SetActive(true);
                    if (canvasLogin != null) canvasLogin.SetActive(false);
                }
                else
                {
                    mensaje.text = "Usuario o contraseña incorrectos";
                }
            }
        }
    }
}