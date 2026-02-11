using TMPro;
using UnityEngine;
using Wordle.Board;

namespace Wordle.Core
{
    public class WordGame : MonoBehaviour
    {
        [SerializeField] TextMeshPro guessInputField;
        [SerializeField] BoardGenerator boardGenerator;
        private int maxAttempts = 6;
        private int currentAttempt = 0;
        private string targetWord;

        void Awake()
        {
            if (boardGenerator == null)
            {
                boardGenerator = FindFirstObjectByType<BoardGenerator>();
                if (boardGenerator == null)
                {
                    Debug.LogError(
                        $"{name} ({GetType().Name}): BoardGenerator not found; cannot retrieve target word.");
                    return;
                }
            }

            targetWord = boardGenerator.GetSecretWord();
        }

        void Update()
        {
            guessInputField.text = guessInputField.text.ToUpper();
        }

        public void CheckGuess()
        {
            if (guessInputField == null)
            {
                Debug.LogError($"{name} ({GetType().Name}): Guess input field not assigned.");
                return;
            }

            string guess = guessInputField.text;

            if (string.IsNullOrEmpty(guess))
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Guess is empty.");
                return;
            }

            if (guess.Length != targetWord.Length)
            {
                Debug.LogWarning(
                    $"{name} ({GetType().Name}): Guess length {guess.Length} does not match target word length {targetWord.Length}.");
                return;
            }

            if (guess == targetWord)
            {
                Debug.Log("Congratulations! You've guessed the word!");
                return;
            } 

            currentAttempt++;
            if (currentAttempt >= maxAttempts)
            {
                Debug.Log("No more attempts left!");
                return;
            }
        }
    }
}
