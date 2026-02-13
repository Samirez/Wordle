using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Data;
using Mono.Data.Sqlite;

public class PlayerRecordsTest
{
    private string lastPlayerName;
    private int lastScore;
    private float lastTime;
    private bool hasInsert;
    
    [Test]
    public void DatabaseFileExists()
    {
        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        Assert.IsTrue(System.IO.File.Exists(filePath), $"Database file not found at: {filePath}");
        Assert.IsFalse(string.IsNullOrEmpty(filePath), "Database file path is null or empty.");
    }

   [Test]
   [TestCase("TestPlayer", 100, 30.5f)]
   [TestCase("AnotherPlayer", 150, 25.0f)]
   [TestCase("Player3", 200, 20.0f)]
   [TestCase("", 50, 15.0f, TestName = "SaveRecord_Allows_Empty_PlayerName")] // Edge case: empty player name
   [TestCase("PlayerWithAVeryLongNameExceedingNormalLimits", 75, 10.0f)] // Edge case: very long player name
    public void SaveRecordTest(string playerName, int score, float time)
    {
        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        string connectionString = "URI=file:" + filePath;
        using var connection = new SqliteConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Records (Id INTEGER PRIMARY KEY AUTOINCREMENT, PlayerName TEXT, Score INTEGER, Time FLOAT)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO Records (PlayerName, Score, Time) VALUES (@name, @score, @time)";
                command.Parameters.AddWithValue("@name", playerName);
                command.Parameters.AddWithValue("@score", score);
                command.Parameters.AddWithValue("@time", time);
            Assert.DoesNotThrow(() => command.ExecuteNonQuery(), "Failed to save record to database.");
            lastPlayerName = playerName;
            lastScore = score;
            lastTime = time;
            hasInsert = true;
    }

    [Test]
    public void GetAllRecordsTest()
    {
        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        string connectionString = "URI=file:" + filePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT PlayerName, Score, Time FROM Records ORDER BY Score DESC, Time ASC";
        using var reader = command.ExecuteReader();
        Assert.IsTrue(reader.HasRows, "No records found in the database.");
        while (reader.Read())
        {
            string playerName = reader.GetString(0);
            int score = reader.GetInt32(1);
            float time = reader.GetFloat(2);
            Assert.IsFalse(string.IsNullOrEmpty(playerName), "Player name is null or empty.");
            Assert.GreaterOrEqual(score, 0, "Score is negative.");
            Assert.GreaterOrEqual(time, 0f, "Time is negative.");
        }
    }

    [TearDown]
    public void RemoveInsertedRecord()
    {
        if (!hasInsert)
        {
            return;
        }

        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        string connectionString = "URI=file:" + filePath;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Records WHERE PlayerName = @name AND Score = @score AND Time = @time";
        command.Parameters.AddWithValue("@name", lastPlayerName);
        command.Parameters.AddWithValue("@score", lastScore);
        command.Parameters.AddWithValue("@time", lastTime);
        command.ExecuteNonQuery();
        hasInsert = false;
    }
}
