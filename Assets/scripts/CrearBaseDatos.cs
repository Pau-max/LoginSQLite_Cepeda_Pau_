using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.IO;

[DefaultExecutionOrder(-100)]
public class CrearBaseDatos : MonoBehaviour
{
    private void Start()
    {
        // Definimos la ruta hacia Assets/Plugins
        string directoryPath = Path.Combine(Application.dataPath, "Plugins");
        string dbPath = Path.Combine(directoryPath, "UserAndInventory.sqlite");

        // Aseguramos que la carpeta Plugins exista
        if (!Directory.Exists(directoryPath)) 
        {
            Directory.CreateDirectory(directoryPath);
        }

        Debug.Log("Base de datos en: " + dbPath);
        if (!File.Exists(dbPath)) File.Create(dbPath).Close();

        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + dbPath))
        {
            dbConnection.Open();
            using (IDbCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Usuarios (
                        id INTEGER PRIMARY KEY AUTOINCREMENT, 
                        usuario TEXT UNIQUE, 
                        password TEXT
                    );";
                dbCommand.ExecuteNonQuery();
            }
        }
    }
}