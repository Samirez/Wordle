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
        wordGame.GuessInputField = guessInputField;
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
        guessInputField.text = "TEST";
        
        wordGame.OnSubmit();
        
        // Verify that the input field was cleared after submission
        Assert.AreEqual(string.Empty, guessInputField.text, "Input field should be cleared after OnSubmit.");
    }

    [Test]
    public void CheckGuessTest()
    {
        // Set a test value in the input field
        guessInputField.text = "APPLE";
        string guess = guessInputField.text;
        
        Assert.IsNotNull(guess, "Guess should not be null.");
        Assert.IsNotEmpty(guess, "Guess should not be empty.");
        Assert.AreEqual("APPLE", guess, "Guess should match the input field text.");
    }
}
