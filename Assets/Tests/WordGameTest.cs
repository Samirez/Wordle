using NUnit.Framework;
using UnityEngine;
using Wordle.Core;
using TMPro;


public class WordGameTest
{
    private GameObject gameObject;
    private WordGame wordGame;
    private TMP_InputField guessInputField;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("WordGameTestObject");
        wordGame = gameObject.AddComponent<WordGame>();
        guessInputField = gameObject.AddComponent<TMP_InputField>();
        
        // Use reflection to set the private guessInputField since the property is read-only
        var guessInputFieldField = typeof(WordGame).GetField("guessInputField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        guessInputFieldField.SetValue(wordGame, guessInputField);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void GetPlayTimeTest()
    {
        float playTime = wordGame.GetPlayTime();
        Assert.AreEqual(0.0f, playTime, 0.01f, "Initial play time should be zero.");
    }

    [Test]
    public void OnSubmitGuessTest()
    {
        // Use reflection to set the private targetWord field so CheckGuess has a valid target
        var targetWordField = typeof(WordGame).GetField("targetWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        targetWordField.SetValue(wordGame, "APPLE");
        
        guessInputField.text = "TEST";
        
        wordGame.OnSubmit();
        
        // Verify that the input field was cleared after submission
        Assert.AreEqual(string.Empty, guessInputField.text, "Input field should be cleared after OnSubmit.");
        
        // Verify that the guess was processed by checking currentAttempt was incremented
        var currentAttemptField = typeof(WordGame).GetField("currentAttempt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        int currentAttempt = (int)currentAttemptField.GetValue(wordGame);
        Assert.AreEqual(1, currentAttempt, "Current attempt should be incremented to 1 after submitting an incorrect guess.");
    }

    [Test]
    public void CheckGuessTest()
    {
        // Use reflection to set the private targetWord field for testing
        var targetWordField = typeof(WordGame).GetField("targetWord", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        targetWordField.SetValue(wordGame, "APPLE");
        
        // Set a test value in the input field
        guessInputField.text = "APPLE";
        
        // Call CheckGuess to evaluate the guess
        wordGame.CheckGuess();
        
        // Get the private isGameOver field to verify the guess was correct
        var isGameOverField = typeof(WordGame).GetField("isGameOver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool isGameOver = (bool)isGameOverField.GetValue(wordGame);
        
        // Assert that the correct guess triggered game over
        Assert.IsTrue(isGameOver, "Game should be over after guessing the correct word 'APPLE'.");
    }
}
