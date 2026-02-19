using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InventoryRepository : MonoBehaviour
{
    private string dbPath;

    void Awake()
    {
        // Construcción de la ruta hacia Assets/Plugins
        string filePath = Path.Combine(Application.dataPath, "Plugins", "UserAndInventory.sqlite");
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
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Items (itemID INTEGER PRIMARY KEY, itemName TEXT, spriteName TEXT);
                    CREATE TABLE IF NOT EXISTS Inventory (
                        userID INTEGER, 
                        itemID INTEGER, 
                        itemQuantity INTEGER, 
                        PRIMARY KEY(userID, itemID)
                    );";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT OR IGNORE INTO Items (itemID, itemName, spriteName) VALUES (1, 'Tostada', 'Toast'), (2, 'Espada', 'Espada');";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void SaveOrUpdateItem(int uID, int iID, int qty)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Inventory (userID, itemID, itemQuantity) VALUES (@u, @i, @q) 
                                   ON CONFLICT(userID, itemID) DO UPDATE SET itemQuantity = itemQuantity + @q";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.Parameters.Add(new SqliteParameter("@q", qty));
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
                        list.Add(new InventoryEntry { 
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

    public void RemoveOneItem(int uID, int iID)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Inventory SET itemQuantity = itemQuantity - 1 
                                    WHERE userID = @u AND itemID = @i AND itemQuantity > 0;
                                    DELETE FROM Inventory WHERE userID = @u AND itemID = @i AND itemQuantity <= 0;";
                cmd.Parameters.Add(new SqliteParameter("@u", uID));
                cmd.Parameters.Add(new SqliteParameter("@i", iID));
                cmd.ExecuteNonQuery();
            }
        }
    }
}