using UnityEngine;
using NUnit.Framework;
using Wordle.Core;

public class GameManagerTest
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private AudioSource audioSource;

    [SetUp]
    public void SetUp()
    {
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        audioSource = gameManagerObject.AddComponent<AudioSource>();
    }

    [TearDown]
    public void TearDown()
    {
        if (gameManagerObject != null)
        {
            Object.DestroyImmediate(gameManagerObject);
        }
    }

    [Test]
    public void PlaySoundtrackTest()
    {
        Assert.IsNotNull(audioSource, "GameManager must have an AudioSource for the test.");

        // Test that the soundtrack is not playing initially
        Assert.IsFalse(audioSource.isPlaying, "Soundtrack should not be playing initially.");

        // Play the soundtrack and test that it is playing
        gameManager.PlaySoundtrack();
        Assert.IsTrue(audioSource.isPlaying, "Soundtrack should be playing after calling PlaySoundtrack.");

        // Stop the soundtrack and test that it is not playing
        gameManager.StopSoundtrack();
        Assert.IsFalse(audioSource.isPlaying, "Soundtrack should not be playing after calling StopSoundtrack.");
    }

    [Test]
    public void StopSoundtrackTest()
    {
        // Play the soundtrack to ensure it is playing before stopping
        gameManager.PlaySoundtrack();
        Assert.IsTrue(audioSource.isPlaying, "Soundtrack should be playing before calling StopSoundtrack.");

        // Stop the soundtrack and test that it is not playing
        gameManager.StopSoundtrack();
        Assert.IsFalse(audioSource.isPlaying, "Soundtrack should not be playing after calling StopSoundtrack.");
    }

    [Test]
    public void OnDestroyTest()
    {
        // Ensure the instance is set
        Assert.AreEqual(gameManager, GameManager.Instance, "GameManager instance should be set to the created instance.");

        // Destroy the GameManager and test that the instance is null
        Object.DestroyImmediate(gameManagerObject);
        gameManagerObject = null; // Prevent double-destroy in TearDown
        Assert.IsNull(GameManager.Instance, "GameManager instance should be null after destruction.");
    }
}