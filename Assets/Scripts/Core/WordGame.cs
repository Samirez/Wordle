using UnityEngine;
using System;
using System.Collections.Generic;

namespace Wordle.Core
{
    public class WordGame : MonoBehaviour
    {
        private string secretWord;
        public string WordReader()
        {
            string[] allWords = System.IO.File.ReadAllLines(Application.dataPath + "/WordList/wordle_ord.txt");
            System.Random rand = new System.Random();
            // check if the file is read correctly
            Debug.Log(allWords[rand.Next(allWords.Length)]);
            return allWords[rand.Next(allWords.Length)];
        }

        public void GenerateMap()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    
                }
            }
        }

        void Start()
        {
            secretWord = WordReader();
            GenerateMap();
        }
    }
}
