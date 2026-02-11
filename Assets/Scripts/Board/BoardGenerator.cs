using UnityEngine;
using System.Collections.Generic;

namespace Wordle.Board
{   
    public class BoardGenerator : MonoBehaviour
    {
       [SerializeField] private GameObject tilePrefab;
       public int gridWidth = 5;
       public int gridHeight = 6;
       public float tileSize = 1.0f;
       public Cell[,] board = new Cell[5, 6];

       void Start()
        {
            GenerateBoard();
        }

        public void GenerateBoard()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.parent = transform;
                    board[x, y] = tile.GetComponent<Cell>();
                }
            }
        }
    }
}
