using UnityEngine;
using Mono.Data.Sqlite;

public class PlayerRecords : MonoBehaviour
{
    void Start()
    {
        string connectionString = "URI=file:" + Application.persistentDataPath + "/PlayerRecords.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, PlayerName TEXT, Score INTEGER, Time FLOAT)";
        command.ExecuteNonQuery();
    }

    public void SaveRecord(string playerName, int score, float time)
    {
        string connectionString = "URI=file:" + Application.persistentDataPath + "/PlayerRecords.db";
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
