using UnityEngine;
using NUnit.Framework;
using Wordle.Core;

public class GameManagerTest
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private AudioSource audioSource;

    private void InvokePrivateMethod(object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method, $"{target.GetType().Name}.{methodName} method not found - check method visibility or name.");
        method.Invoke(target, null);
    }

    [SetUp]
    public void SetUp()
    {
        gameManagerObject = new GameObject("GameManager");
        audioSource = gameManagerObject.AddComponent<AudioSource>();
        
        // Create a dummy AudioClip for testing playback
        audioSource.clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
        
        gameManager = gameManagerObject.AddComponent<GameManager>();
        
        // Manually invoke Awake since it's not called automatically in edit mode tests
        InvokePrivateMethod(gameManager, "Awake");
    }

    [TearDown]
    public void TearDown()
    {
        if (gameManagerObject != null)
        {
            // Manually invoke OnDestroy to clear the static Instance
            if (gameManager != null)
            {
                InvokePrivateMethod(gameManager, "OnDestroy");
            }
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
        // Directly start the audio source to ensure it's playing
        audioSource.Play();
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

        // Manually invoke OnDestroy to clear the static Instance before destroying
        InvokePrivateMethod(gameManager, "OnDestroy");
        
        // Destroy the GameManager and test that the instance is null
        Object.DestroyImmediate(gameManagerObject);
        gameManagerObject = null; // Prevent double-destroy in TearDown
        Assert.IsNull(GameManager.Instance, "GameManager instance should be null after destruction.");
    }
}