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
        
        // Set the internal guessInputField directly (visible via InternalsVisibleTo)
        wordGame.guessInputField = guessInputField;
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
        // Set the internal targetWord field directly (visible via InternalsVisibleTo)
        wordGame.targetWord = "APPLE";

        Assert.AreEqual(0, wordGame.currentAttempt, "Current attempt should start at 0.");
        
        guessInputField.text = "WRONG";
        
        wordGame.OnSubmit();
        
        // Verify that the input field was cleared after submission
        Assert.AreEqual(string.Empty, guessInputField.text, "Input field should be cleared after OnSubmit.");
        
        // Verify that the guess was processed by checking currentAttempt was incremented
        Assert.AreEqual(1, wordGame.currentAttempt, "Current attempt should be incremented to 1 after submitting an incorrect guess.");
    }

    [Test]
    public void CheckGuessTest()
    {
        // Set the internal targetWord field directly (visible via InternalsVisibleTo)
        wordGame.targetWord = "APPLE";
        
        // Set a test value in the input field
        guessInputField.text = "APPLE";
        
        // Call CheckGuess to evaluate the guess
        wordGame.CheckGuess();
        
        // Verify that the correct guess triggered game over
        Assert.IsTrue(wordGame.isGameOver, "Game should be over after guessing the correct word 'APPLE'.");
    }
}
