using TMPro;
using UnityEngine;
using Wordle.Board;

namespace Wordle.Core
{
    public class WordGame : MonoBehaviour
    {
        [SerializeField] TMP_InputField guessInputField;
        [SerializeField] BoardGenerator boardGenerator;
        [SerializeField] TextMeshProUGUI attemptsText;
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

            if (!string.IsNullOrEmpty(targetWord))
            {
                targetWord = targetWord.ToUpperInvariant();
            }
        }

        public void OnSubmit()
        {
            if (guessInputField == null)
            {
                return;
            }

            string currentText = guessInputField.text;
            string upperText = currentText.ToUpperInvariant();
            if (!string.Equals(currentText, upperText, System.StringComparison.Ordinal))
            {
                guessInputField.text = upperText;
            }

            CheckGuess();

            guessInputField.text = string.Empty;

            if (attemptsText != null)
            {
                attemptsText.text = $"{maxAttempts - currentAttempt}";
            }
        }

        public void CheckGuess()
        {
            if (guessInputField == null)
            {
                Debug.LogError($"{name} ({GetType().Name}): Guess input field not assigned.");
                return;
            }

            if (string.IsNullOrEmpty(targetWord))
            {
                Debug.LogError($"{name} ({GetType().Name}): Target word is not initialized.");
                return;
            }

            string guess = guessInputField.text;

            if (string.IsNullOrEmpty(guess))
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Guess is empty.");
                return;
            }

            if (string.Equals(guess, targetWord, System.StringComparison.Ordinal))
            {
                Debug.Log("Congratulations! You've guessed the word!");
                ApplyGuessToBoard(guess);
                return;
            } 

            ApplyGuessToBoard(guess);

            currentAttempt++;
            if (currentAttempt >= maxAttempts)
            {
                Debug.Log("No more attempts left!");
                return;
            }
        }

        private void ApplyGuessToBoard(string guess)
        {
            if (boardGenerator == null || boardGenerator.board == null)
            {
                return;
            }

            int row = (boardGenerator.gridHeight - 1) - currentAttempt;
            if (row < 0 || row >= boardGenerator.gridHeight)
            {
                return;
            }

            var remaining = new System.Collections.Generic.Dictionary<char, int>();
            for (int i = 0; i < targetWord.Length; i++)
            {
                char targetChar = targetWord[i];
                if (remaining.TryGetValue(targetChar, out int count))
                {
                    remaining[targetChar] = count + 1;
                }
                else
                {
                    remaining[targetChar] = 1;
                }
            }

            int maxX = Mathf.Min(guess.Length, targetWord.Length, boardGenerator.board.GetLength(0));

            for (int x = 0; x < maxX; x++)
            {
                char guessChar = guess[x];
                if (guessChar == targetWord[x] && remaining.TryGetValue(guessChar, out int count))
                {
                    remaining[guessChar] = Mathf.Max(0, count - 1);
                }
            }

            for (int x = 0; x < maxX; x++)
            {
                char guessChar = guess[x];
                string type = "absent";

                if (guessChar == targetWord[x])
                {
                    type = "correct";
                }
                else if (remaining.TryGetValue(guessChar, out int count) && count > 0)
                {
                    type = "present";
                    remaining[guessChar] = count - 1;
                }

                Cell cell = boardGenerator.board[x, row];
                if (cell != null)
                {
                    string existingLetter = cell.letter != null ? cell.letter.text : string.Empty;
                    cell.Setup(x, row, type: type, letterChar: existingLetter);
                }
            }
        }
    }
}
