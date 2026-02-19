using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO; // Añadido para Path

public class Registro : MonoBehaviour
{
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContraseña;
    public Button botonRegistrar;
    public TextMeshProUGUI mensaje;

    void Start()
    {
        botonRegistrar.onClick.AddListener(RegistrarNuevoUsuario);
    }

    void RegistrarNuevoUsuario()
    {
        string usuario = inputUsuario.text.Trim();
        string contraseña = inputContraseña.text.Trim();
        
        // Nueva ruta a Plugins
        string rutaDB = Path.Combine(Application.dataPath, "Plugins", "UserAndInventory.sqlite");

        if (contraseña.Length < 8 || string.IsNullOrEmpty(usuario))
        {
            mensaje.text = "Contraseña muy corta o usuario vacío";
            return;
        }

        using (var conexion = new SqliteConnection("URI=file:" + rutaDB))
        {
            conexion.Open();
            string consulta = "INSERT INTO Usuarios (usuario, password) VALUES (@usuario, @password)";
            using (var comando = new SqliteCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@usuario", usuario);
                comando.Parameters.AddWithValue("@password", contraseña);

                try
                {
                    comando.ExecuteNonQuery();
                    mensaje.text = "¡Usuario " + usuario + " registrado con éxito!";
                }
                catch (SqliteException e)
                {
                    mensaje.text = e.Message.Contains("UNIQUE") ? "Ese nombre de usuario ya existe" : "Error de BD";
                }
            }
        }
    }
}