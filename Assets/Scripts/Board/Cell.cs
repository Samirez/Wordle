using UnityEngine;
using TMPro;


namespace Wordle.Board
{
    public enum CellType
    {
        Normal,
        Correct,
        Present,
        Absent,
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Cell : MonoBehaviour
    {
        public int x, y;
        public CellType type = CellType.Normal;
        public TMP_Text letter;
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (letter == null)
            {
                letter = GetComponentInChildren<TMP_Text>(true);
            }
        }

        public void Setup(int x, int y, string letterChar = "", CellType type = CellType.Normal)
        {
            this.x = x;
            this.y = y;
            this.type = type;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.color = type switch
            {
                CellType.Correct => Color.green,
                CellType.Present => Color.yellow,
                CellType.Absent => Color.gray,
                _ => Color.white,
            };
            if (letter != null)
            {
                letter.text = letterChar?.ToUpper() ?? "";
            }
        }
    }
}
