using UnityEngine;
using TMPro;


namespace Wordle.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Cell : MonoBehaviour
    {
        public int x, y;
        public string type = "normal"; // normal, correct, present, absent
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

        public void Setup(int x, int y, string letterChar = "", string type = "normal")
        {
            this.x = x;
            this.y = y;
            this.type = type;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.color = type switch
            {
                "correct" => Color.green,
                "present" => Color.yellow,
                "absent" => Color.gray,
                _ => Color.white,
            };
            if (letter != null)
            {
                letter.text = letterChar?.ToUpper() ?? "";
            }
        }
    }
}
