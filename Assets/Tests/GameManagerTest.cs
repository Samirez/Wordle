using UnityEngine;
using NUnit.Framework;
using Wordle.Core;

public class GameManagerTest
{
    [Test]
    public void PlaySoundtrackTest()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();

        // Test that the soundtrack is not playing initially
        Assert.IsFalse(gameManager.GetComponent<AudioSource>().isPlaying, "Soundtrack should not be playing initially.");

        // Play the soundtrack and test that it is playing
        gameManager.PlaySoundtrack();
        Assert.IsTrue(gameManager.GetComponent<AudioSource>().isPlaying, "Soundtrack should be playing after calling PlaySoundtrack.");

        // Stop the soundtrack and test that it is not playing
        gameManager.StopSoundtrack();
        Assert.IsFalse(gameManager.GetComponent<AudioSource>().isPlaying, "Soundtrack should not be playing after calling StopSoundtrack.");
    }

    [Test]
    public void StopSoundtrackTest()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();

        // Play the soundtrack to ensure it is playing before stopping
        gameManager.PlaySoundtrack();
        Assert.IsTrue(gameManager.GetComponent<AudioSource>().isPlaying, "Soundtrack should be playing before calling StopSoundtrack.");

        // Stop the soundtrack and test that it is not playing
        gameManager.StopSoundtrack();
        Assert.IsFalse(gameManager.GetComponent<AudioSource>().isPlaying, "Soundtrack should not be playing after calling StopSoundtrack.");
    }

    [Test]
    public void OnDestroyTest()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();

        // Ensure the instance is set
        Assert.AreEqual(gameManager, GameManager.Instance, "GameManager instance should be set to the created instance.");

        // Destroy the GameManager and test that the instance is null
        Object.DestroyImmediate(gameManagerObject);
        Assert.IsNull(GameManager.Instance, "GameManager instance should be null after destruction.");
    }
}