using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;



public class PlayerRecordsTest
{
    
    // A Test behaves as an ordinary method
    [Test]
    public void PlayerRecordsSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    [Test]
    public bool DatabaseFileExists()
    {
        string filePath = Application.dataPath + "/Data_storage/PlayerRecords.db";
        return System.IO.File.Exists(filePath);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerRecordsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
