using UnityEngine;
using TMPro;


namespace Wordle.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Cell : MonoBehaviour
    {
        public int x, y;
        public string type = "normal"; // normal, correct, present, absent
        public TextMeshPro letter;
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Setup(int x, int y, string type = "normal")
        {
            this.x = x;
            this.y = y;
            this.type = type;

            spriteRenderer.color = type switch
            {
                "correct" => Color.green,
                "present" => Color.yellow,
                "absent" => Color.gray,
                _ => Color.white,
            };

            if (letter != null)
            {
                letter.text = type;
            }
        }
    }
}
