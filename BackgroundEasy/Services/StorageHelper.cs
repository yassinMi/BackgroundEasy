using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Services
{
    public class StorageHelper
    {


        SQLiteConnection Connection { get; set; }

        public StorageHelper(string connectionString)
        {
            Connection = new SQLiteConnection(connectionString);
            Connection.Open();
            CreateTable();
        }

        public void CreateTable()
        {
            using (var command = new SQLiteCommand("create table IF NOT EXISTS Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, SKU TEXT Unique )", Connection))
            {
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// add a sku to the table, returns false in case of duplicated item
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public bool AddItem(string sku)
        {
            // Check if the item already exists in the table
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Items WHERE SKU = @sku", Connection))
            {
                command.Parameters.AddWithValue("@sku", sku);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                    return false;
            }

            // Insert the item into the table
            using (var command = new SQLiteCommand("INSERT INTO Items (SKU) VALUES (@sku)", Connection))
            {
                command.Parameters.AddWithValue("@sku", sku);
                command.ExecuteNonQuery();
            }

            return true;
        }

        public int AddItems(IEnumerable<string> Images)
        {
            int addedCount = 0;

            using (var transaction = Connection.BeginTransaction())
            {
                using (var command = new SQLiteCommand("INSERT OR Ignore INTO Items (SKU) VALUES (@sku)", Connection))
                {
                    foreach (string sku in Images)
                    {
                        command.Parameters.AddWithValue("@sku", sku);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            addedCount++;
                    }
                   
                }
                

                transaction.Commit();
            }

            return addedCount;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearTable()
        {
            using (var command = new SQLiteCommand("DELETE FROM Items", Connection))
            {
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// returns the items in the order they were added with <see cref="AddItem(string)"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetItems()
        {
            var items = new List<string>();

            using (var command = new SQLiteCommand("SELECT SKU FROM Items ORDER BY Id", Connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string sku = reader.GetString(0);
                    items.Add(sku);
                }
            }

            return items;
        }

        public int GetItemsCount()
        {
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Items", Connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }

    }
}
