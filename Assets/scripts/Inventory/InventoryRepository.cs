using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InventoryRepository : MonoBehaviour
{
    private string dbPath;
    private const int MAX_SLOTS = 15;

    void Awake()
    {
        string folderPath = Path.Combine(Application.dataPath, "Plugins");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        string filePath = Path.Combine(folderPath, "RPG_Database.db");
        dbPath = "URI=file:" + filePath;

        InitializeDB();
    }

    private void InitializeDB()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Items (
                        itemID INTEGER PRIMARY KEY, 
                        itemName TEXT, 
                        description TEXT, 
                        spriteName TEXT, 
                        rarity INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS Inventory (
                        userID INTEGER, 
                        itemID INTEGER, 
                        itemQuantity INTEGER, 
                        PRIMARY KEY(userID, itemID)
                    );";
                cmd.ExecuteNonQuery();

                // Datos iniciales de prueba
                cmd.CommandText = "INSERT OR IGNORE INTO Items VALUES (1, 'Pocion', 'Cura HP', 'Pocion', 1), (2, 'Espada', 'Ataque +5', 'Espada', 2);";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public bool SaveOrUpdateItem(int uID, int iID, int qty)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            // Lógica de guardado o incremento (ON CONFLICT)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Inventory (userID, itemID, itemQuantity) VALUES (@u, @i, @q) 
                                   ON CONFLICT(userID, itemID) DO UPDATE SET itemQuantity = itemQuantity + @q";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.Parameters.Add(new SqliteParameter("@q", qty));
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }

    public void RemoveOneItem(int uID, int iID)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                // Resta 1 y borra si llega a 0
                cmd.CommandText = @"
                    UPDATE Inventory SET itemQuantity = itemQuantity - 1 
                    WHERE userID = @u AND itemID = @i AND itemQuantity > 0;
                    DELETE FROM Inventory WHERE userID = @u AND itemID = @i AND itemQuantity <= 0;";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<InventoryEntry> GetInventory(int uID)
    {
        List<InventoryEntry> list = new List<InventoryEntry>();
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT i.itemID, i.itemName, inv.itemQuantity, i.spriteName 
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
                            itemQuantity = reader.GetInt32(2),
                            spriteName = reader.GetString(3)
                        });
                    }
                }
            }
        }
        return list;
    }
}