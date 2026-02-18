using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Wordle.Board
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BoardGenerator : MonoBehaviour
    {
        private const char EmptyCell = (char)33;
        [SerializeField] private GameObject tilePrefab;
        public int gridWidth = 5;
        public int gridHeight = 6;
        public float tileSize = 1.0f;
        [SerializeField] private string boardSortingLayer = "UI";
        [SerializeField] private int boardSortingOrder = 5;
        [SerializeField] private int letterSortingOrderOffset = 1;
        readonly Random random = new();
        public Cell[,] board;
        public ObservableCollection<ObservableCollection<char>> Chars { get; set; }
        public List<string> Words { get; private set; } = new List<string>();
        private string secretWord;

        public enum WordDirection
        {
            Horizontal,
            Vertical
        }

        void Awake()
        {
            if (tilePrefab == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Tile prefab not assigned; board generation may fail.");
            }

            try
            {
                secretWord = WordReader();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{name} ({GetType().Name}): Failed to read secret word: {ex.Message}");
                secretWord = "";
            }

            if (string.IsNullOrEmpty(secretWord))
            {
                Debug.LogError($"{name} ({GetType().Name}): Secret word is empty; board generation may be disabled.");
                secretWord = "APPLE";
            }
        }

        public void InitializeBoard()
        {
            GenerateBoard();
            FillWithEmptyChars();
            WriteWords(Words);
            FillRestOfSpace();
        }

        public void FillWithEmptyChars()
        {
            Chars = new();
            for (int x = 0; x < gridWidth; x++)
            {
                Chars.Add(new());
                for (int y = 0;  y < gridHeight; y ++)
                {
                    Chars[x].Add(EmptyCell); 
                }
            }
        }

        public void WriteWords(List<string> words)
        {
            // first write the secret word to ensure it's on the board
            var secretStartX = random.Next(0, gridWidth);
            var secretStartY = random.Next(0, gridHeight);            
            var secretWordDir = (WordDirection) random.Next(0, 2);
            WriteWord(secretWord, secretStartX, secretStartY, secretWordDir, true);

            foreach(string word in words)
            {
                var startx = random.Next(0, gridWidth);
                var starty =  random.Next(0, gridHeight);
                var worddir = (WordDirection) random.Next(0, 2);                
                if (WriteWord(word, startx, starty, worddir, false))
                    WriteWord(word, startx, starty, worddir, true);
            }
        }

        public bool WriteWord(IEnumerable<char> word, 
                        int startX, 
                        int startY,
                        WordDirection direction,
                        bool commit)
        {
            //Start position
            int x = startX;
            int y = startY;
            int wordLength = word.Count();

            if (direction == WordDirection.Horizontal && wordLength > gridWidth)
            {
                return false;
            }

            if (direction == WordDirection.Vertical && wordLength > gridHeight)
            {
                return false;
            }
            
            //Clamp position
                if(direction == WordDirection.Horizontal)
                    x = Math.Clamp(startX, 0, gridWidth - wordLength);
                if(direction == WordDirection.Vertical)
                    y = Math.Clamp(startY, 0, gridHeight - wordLength);
            
            //Add a letter
            foreach(char ch in word)
            {
                //Check if there is space or that the letters match up
                if (!(Chars[x][y] == ch || Chars[x][y] == EmptyCell))
                    return false;

                //Only commit after checking if there's space left, to not overwrite other letters
                if (commit)
                    Chars[x][y] = ch;
                    
                if(direction == WordDirection.Horizontal)
                    x++;
                if(direction == WordDirection.Vertical)
                    y++;
            }            
            return true;
        }
        
        public void FillRestOfSpace()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (Chars[x][y] == EmptyCell)
                        Chars[x][y] = (char)random.Next(65, 91);
                }
            }
            ApplyCharsToBoard();
        }

        private void ApplyCharsToBoard()
        {
            if (board == null || Chars == null)
            {
                return;
            }

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Cell cell = board[x, y];
                    if (cell == null)
                    {
                        continue;
                    }

                    char ch = Chars[x][y];
                    string letterChar = ch == EmptyCell ? "" : ch.ToString();
                    cell.Setup(x, y, letterChar: letterChar, type: cell.type);
                }
            }
        }

        public string WordReader()
        {
            try
            {
                string[] allWords = System.IO.File.ReadAllLines(Application.dataPath + "/WordList/wordle_ord.txt");
                
                if (allWords.Length == 0)
                {
                    Debug.LogError("Word list is empty");
                    return string.Empty;
                }
                string selectedWord = allWords[random.Next(allWords.Length)];

                #if UNITY_EDITOR
                    Debug.Log($"Selected word: {selectedWord}");
                #endif
                return selectedWord.ToUpperInvariant();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read word list: {ex.Message}");
                return string.Empty;
            }
        }
        public void GenerateBoard()
        {
            ClearBoard();
            Vector3 origin = transform.position;
            float xOffset = (gridWidth - 1) * 0.5f * tileSize;
            float yOffset = (gridHeight - 1) * 0.5f * tileSize;

            if (tilePrefab == null)
            {
                Debug.LogError($"{name} ({GetType().Name}): Tile prefab not assigned; cannot generate board.");
                return;
            }

            board = new Cell[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 local = new Vector3(x * tileSize - xOffset, y * tileSize - yOffset, 0f);
                    Vector3 position = origin + local;
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.parent = transform;

                    if (tile.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                    {
                        spriteRenderer.sortingLayerName = boardSortingLayer;
                        spriteRenderer.sortingOrder = boardSortingOrder;
                    }

                    var letterText = tile.GetComponentInChildren<TMP_Text>(true);
                    if (letterText != null && letterText.TryGetComponent<Renderer>(out var letterRenderer))
                    {
                        letterRenderer.sortingLayerName = boardSortingLayer;
                        letterRenderer.sortingOrder = boardSortingOrder + letterSortingOrderOffset;
                    }
                    else if (letterText != null)
                    {
                        Debug.LogWarning(
                            $"{name} ({GetType().Name}): '{tile.name}' has TMP_Text '{letterText.name}' but no Renderer; cannot apply sorting order.");
                    }

                    if (!tile.TryGetComponent<Cell>(out var cell))
                    {
                        Debug.LogWarning(
                            $"{name} ({GetType().Name}): '{tile.name}' at ({x}, {y}) is missing a Cell component; adding one.");
                        cell = tile.AddComponent<Cell>();
                    }

                    board[x, y] = cell;
                }
            }
        }

        public void ClearBoard()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
            #if UNITY_EDITOR

                if (!Application.isPlaying)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            #else
                Destroy(child.gameObject);
            #endif
            }

            board = null;
            Chars = null;
        }

        public string GetSecretWord()
        {
            return secretWord;
        }
    }
}
