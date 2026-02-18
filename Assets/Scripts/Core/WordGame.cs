using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Wordle.Board;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Wordle.Core
{
    public class WordGame : MonoBehaviour
    {
        [SerializeField] internal TMP_InputField guessInputField;
        public TMP_InputField GuessInputField => guessInputField;
        
        [SerializeField] BoardGenerator boardGenerator;
        [SerializeField] TextMeshProUGUI attemptsText;
        [SerializeField] TextMeshProUGUI playTimeText;
        private Menu menu;
        
        private int maxAttempts = 6;
        internal int currentAttempt = 0;
        internal string targetWord;
        private float playTime = 0.0f;
        internal bool isGameOver = false;

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

            menu = FindFirstObjectByType<Menu>();
            
        }

        void Update()
        {
            if (isGameOver)
            {
                return;
            }

            playTime += Time.deltaTime;

            if (playTimeText != null)
            {
                playTimeText.text = $"{playTime:F1}";
            }
        }

        public float GetPlayTime()
        {
            return playTime;
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
                TriggerGameOver();
                return;
            } 

            ApplyGuessToBoard(guess);

            currentAttempt++;
            if (currentAttempt >= maxAttempts)
            {
                Debug.Log("No more attempts left!");
                TriggerGameOver();
                return;
            }
        }

        private void TriggerGameOver()
        {
            if (isGameOver)
            {
                return;
            }

            isGameOver = true;
            if (menu != null)
            {
                StartCoroutine(DelayedGameOver());
            }
        }

        private IEnumerator DelayedGameOver()
        {
            yield return new WaitForSeconds(5.0f);
            if (menu != null)
            {
                menu.ShowGameOver();
            }
        }

        private void ApplyGuessToBoard(string guess)
        {
            if (boardGenerator == null || boardGenerator.board == null)
            {
                return;
            }

            int row = boardGenerator.gridHeight - 1 - currentAttempt;
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
