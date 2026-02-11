using UnityEngine;

namespace Wordle.Board
{   
    public class BoardGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        public int gridWidth = 5;
        public int gridHeight = 6;
        public float tileSize = 1.0f;
        
        public Cell[,] board;

        public void GenerateBoard()
        {
            board = new Cell[gridWidth, gridHeight];
            Vector3 origin = transform.position;
            float xOffset = (gridWidth - 1) * 0.5f * tileSize;
            float yOffset = (gridHeight - 1) * 0.5f * tileSize;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 local = new Vector3(x * tileSize - xOffset, y * tileSize - yOffset, 0f);
                    Vector3 position = origin + local;
                    GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.parent = transform;
                    board[x, y] = tile.GetComponent<Cell>();
                }
            }
        }
    }
}
