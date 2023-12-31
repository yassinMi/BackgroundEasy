﻿using BackgroundEasy.Model;
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
            using (var command = new SQLiteCommand("create table IF NOT EXISTS Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, ImagePath TEXT Unique )", Connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command2 = new SQLiteCommand(@" CREATE TABLE IF NOT EXISTS Presets ( Id INTEGER PRIMARY KEY AUTOINCREMENT, ImagePath TEXT, SolidColorHex TEXT, Name TEXT UNIQUE )", Connection))
            {
                command2.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// add a ImagePath to the table, returns false in case of duplicated item
        /// </summary>
        /// <param name="magePath"></param>
        /// <returns></returns>
        public bool AddItem(string imagePath)
        {
            // Check if the item already exists in the table
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Items WHERE ImagePath = @ImagePath", Connection))
            {
                command.Parameters.AddWithValue("@ImagePath", imagePath);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                    return false;
            }

            // Insert the item into the table
            using (var command = new SQLiteCommand("INSERT INTO Items (ImagePath) VALUES (@ImagePath)", Connection))
            {
                command.Parameters.AddWithValue("@ImagePath", imagePath);
                command.ExecuteNonQuery();
            }

            return true;
        }

        public int AddItems(IEnumerable<string> Images)
        {
            int addedCount = 0;

            using (var transaction = Connection.BeginTransaction())
            {
                using (var command = new SQLiteCommand("INSERT OR Ignore INTO Items (ImagePath) VALUES (@ImagePath)", Connection))
                {
                    foreach (string imagePath in Images)
                    {
                        command.Parameters.AddWithValue("@ImagePath", imagePath);
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

            using (var command = new SQLiteCommand("SELECT ImagePath FROM Items ORDER BY Id", Connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string imagePath = reader.GetString(0);
                    items.Add(imagePath);
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



        #region presets


        public void AddPreset(Preset preset)
        {
           
                using (var transaction = Connection.BeginTransaction())
                {
                    using (var command = new SQLiteCommand("INSERT INTO Presets (ImagePath, SolidColorHex, Name) VALUES (@imagePath, @solidColorHex, @name)", Connection))
                    {
                        command.Parameters.AddWithValue("@imagePath", preset.GetStorageImagePathOrPaths());
                        command.Parameters.AddWithValue("@solidColorHex", preset.SolidColorHex);
                        command.Parameters.AddWithValue("@name", preset.Name);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            
        }

        public IEnumerable<Preset> GetPresets()
        {
            var presets = new List<Preset>();

          
                using (var command = new SQLiteCommand("SELECT ImagePath, SolidColorHex, Name FROM Presets", Connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imagePathOrPaths = reader.IsDBNull(0)? null : reader.GetString(0);
                        string solidColorHex = reader.IsDBNull(1) ? null : reader.GetString(1);
                        string name = reader.GetString(2);

                        var preset = new Preset
                        {
                            SolidColorHex = solidColorHex,
                            Name = name
                        };
                    preset.SetStorageImagePathOrPaths(imagePathOrPaths);

                        presets.Add(preset);
                    }
                }
            

            return presets;
        }
        //passes the new preset
        public event EventHandler<Preset> PresetUpdated;

        public void UpdatePreset(Preset oldpreset,Preset newPreset)
        {
            if (oldpreset == null) throw new ArgumentNullException("oldPreset");
            if (newPreset == null) throw new ArgumentNullException("newPreset");
            if (oldpreset.Name != newPreset.Name) throw new Exception("new preset must have the same name");
            if (
                oldpreset.GetStorageImagePathOrPaths() != newPreset.GetStorageImagePathOrPaths() ||
                oldpreset.SolidColorHex != newPreset.SolidColorHex
                )
            {
                using (var transaction = Connection.BeginTransaction())
                {
                    using (var command = new SQLiteCommand("UPDATE Presets SET ImagePath = @imagePath, SolidColorHex = @solidColorHex WHERE Name = @name", Connection))
                    {
                        command.Parameters.AddWithValue("@imagePath", newPreset.GetStorageImagePathOrPaths());
                        command.Parameters.AddWithValue("@solidColorHex", newPreset.SolidColorHex);
                        command.Parameters.AddWithValue("@name", newPreset.Name);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                PresetUpdated?.Invoke(this, newPreset);
            }
            else
            {
                return;
            }

                
            
        }

        public void DeletePreset(string presetName)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                using (var command = new SQLiteCommand("DELETE FROM Presets WHERE Name = @name", Connection))
                {
                    command.Parameters.AddWithValue("@name", presetName);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        #endregion presets

    }
}
