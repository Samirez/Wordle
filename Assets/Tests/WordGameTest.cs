using NUnit.Framework;
using UnityEngine;
using Wordle.Core;
using TMPro;


public class WordGameTest
{
    private GameObject wordGame;
    [Test]
    public void GetPlayTimeTest()
    {
        
        var gameObject = new GameObject("WordGameTestObject");
        try
        {
            var wordGame = gameObject.AddComponent<WordGame>();
            float playTime = wordGame.GetPlayTime();

            Assert.AreEqual(0.0f, playTime, 0.01f, "Initial play time should be zero.");
        }
        finally
        {
            Object.DestroyImmediate(gameObject);
        }
    }

    [Test]
    public void OnSubmitGuessTest()
    {
        var gameObject = new GameObject("WordGameTestObject");
        try
        {
            var wordGame = gameObject.AddComponent<WordGame>();
            var guessInputField = gameObject.AddComponent<TMP_InputField>();
            wordGame.guessInputField = guessInputField;
            guessInputField.text = "TEST";
            wordGame.OnSubmit();
        }
        finally
        {
            Object.DestroyImmediate(gameObject);
        }
    }

    [Test]
    public void CheckGuessTest()
    {
        var gameObject = new GameObject("WordGameTestObject");
        try
        {
            var wordGame = gameObject.AddComponent<WordGame>();
            var guessInputField = gameObject.AddComponent<TMP_InputField>();
            wordGame.guessInputField = guessInputField;
            string guess = guessInputField.text;
            Assert.IsNotNull(guess, "Guess should not be null.");
            Assert.IsNotEmpty(guess, "Guess should not be empty.");
        }
        finally
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}
