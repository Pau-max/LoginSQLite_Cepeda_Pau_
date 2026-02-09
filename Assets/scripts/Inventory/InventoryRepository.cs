using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Collections.Generic;

public class InventoryRepository : MonoBehaviour
{
    private string dbPath;

    void Awake()
    {
        // La base de datos es local y se conserva al cerrar el juego
        dbPath = "URI=file:" + Application.persistentDataPath + "/RPG_Database.db";
        InitializeDB();
    }

    private void InitializeDB()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // 1. Crear tabla Items (según tu diagrama + ampliación rarity)
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Items (
                    itemID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    itemName TEXT, 
                    description TEXT,
                    rarity INTEGER DEFAULT 1
                );";
                cmd.ExecuteNonQuery();

                // 2. Crear tabla Inventory (según tu diagrama)
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Inventory (
                    inventoryID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    userID INTEGER NOT NULL, 
                    itemID INTEGER NOT NULL, 
                    itemQuantity INTEGER DEFAULT 1,
                    UNIQUE(userID, itemID)
                );";
                cmd.ExecuteNonQuery();

                // 3. Insertar items base si la tabla está vacía
                cmd.CommandText = "SELECT COUNT(*) FROM Items";
                if (System.Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    cmd.CommandText = @"INSERT INTO Items (itemName, description, rarity) VALUES 
                        ('Poción Roja', 'Cura 50 HP', 1), 
                        ('Espada de Hierro', 'Ataque +10', 2),
                        ('Escudo Viejo', 'Defensa +5', 1);";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    // CRUD: CREATE / UPDATE (Añadir o modificar cantidad automáticamente)
    public void SaveOrUpdateItem(int uID, int iID, int qty)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // SQL que guarda cambios automáticamente: Si existe suma, si no crea
                cmd.CommandText = @"INSERT INTO Inventory (userID, itemID, itemQuantity) 
                                   VALUES (@u, @i, @q) 
                                   ON CONFLICT(userID, itemID) 
                                   DO UPDATE SET itemQuantity = itemQuantity + @q";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.Parameters.Add(new SqliteParameter("@q", qty));
                cmd.ExecuteNonQuery();
            }
        }
    }

    // CRUD: READ (Cargar inventario dependiente del usuario)
    public List<InventoryEntry> GetUserInventory(int uID)
    {
        List<InventoryEntry> list = new List<InventoryEntry>();
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT i.itemID, i.itemName, inv.itemQuantity 
                                   FROM Inventory inv 
                                   JOIN Items i ON inv.itemID = i.itemID 
                                   WHERE inv.userID = @u";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new InventoryEntry
                        {
                            itemID = reader.GetInt32(0),
                            itemName = reader.GetString(1),
                            itemQuantity = reader.GetInt32(2)
                        });
                    }
                }
            }
        }
        return list;
    }

    // CRUD: DELETE (Eliminar objeto)
    public void DeleteItem(int uID, int iID)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Inventory WHERE userID = @u AND itemID = @i";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.ExecuteNonQuery();
            }
        }
    }
}