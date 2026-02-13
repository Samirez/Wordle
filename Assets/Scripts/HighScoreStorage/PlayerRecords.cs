using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;


namespace Wordle.HighScoreStorage
{   
    public class PlayerRecords : MonoBehaviour
    {
        void Awake()
        {
           if (databaseExists())
           {
               Debug.LogWarning($"{name} ({GetType().Name}): Database file already exists; existing records will be preserved.");
           }
           else
           {
               Debug.Log($"{name} ({GetType().Name}): Database file not found; a new database will be created.");
               string storageDirectory = Application.dataPath + "/Data_storage";
               if (!Directory.Exists(storageDirectory))
               {
                   Directory.CreateDirectory(storageDirectory);
               }

               SqliteConnection.CreateFile(storageDirectory + "/PlayerRecords.db");
           }
        }

        private bool databaseExists()
        {
            string databasePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
            return System.IO.File.Exists(databasePath);
        }

        void Start()
        {
            string connectionString = "URI=file:" + Application.dataPath + "/Data_storage/PlayerRecords.db";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, PlayerName TEXT, Score INTEGER, Time FLOAT)";
            command.ExecuteNonQuery();
        }

        public void SaveRecord(string playerName, int score, float time)
        {
            string connectionString = "URI=file:" + Application.dataPath + "/Data_storage/PlayerRecords.db";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Records (PlayerName, Score, Time) VALUES (@name, @score, @time)";
            command.Parameters.AddWithValue("@name", playerName);
            command.Parameters.AddWithValue("@score", score);
            command.Parameters.AddWithValue("@time", time);
            command.ExecuteNonQuery();
        }
    }
}
