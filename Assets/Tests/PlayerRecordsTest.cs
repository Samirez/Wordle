using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerRecordsTest
{
    
    [Test]
    public void DatabaseFileExists()
    {
        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        Assert.IsTrue(System.IO.File.Exists(filePath), $"Database file not found at: {filePath}");
        Assert.IsFalse(string.IsNullOrEmpty(filePath), "Database file path is null or empty.");
    }
    
}
