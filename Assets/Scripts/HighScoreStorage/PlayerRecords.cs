using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;


namespace Wordle.HighScoreStorage
{   
    public class Record
    {
        public string PlayerName { get; }
        public int Score { get; }
        public float Time { get; }

        public Record(string playerName, int score, float time)
        {
            PlayerName = playerName;
            Score = score;
            Time = time;
        }
    }

    public class PlayerRecords : MonoBehaviour
    {
        private string ConnectionString => "URI=file:" + Application.dataPath + "/Data_storage/PlayerRecords.db";

        private SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

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
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, PlayerName TEXT, Score INTEGER, Time FLOAT)";
            command.ExecuteNonQuery();
        }

        public void SaveRecord(string playerName, int score, float time)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Records (PlayerName, Score, Time) VALUES (@name, @score, @time)";
            command.Parameters.AddWithValue("@name", playerName);
            command.Parameters.AddWithValue("@score", score);
            command.Parameters.AddWithValue("@time", time);
            command.ExecuteNonQuery();
        }

        public List<Record> GetAllRecords()
        {
            var records = new List<Record>();
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT PlayerName, Score, Time FROM Records ORDER BY Score DESC, Time ASC";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string playerName = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0);
                int score = reader.GetInt32(1);
                float time = reader.GetFloat(2);
                records.Add(new Record(playerName, score, time));
                Debug.Log($"Player: {playerName}, Score: {score}, Time: {time}");
            }

            return records;
        }
    }
}
