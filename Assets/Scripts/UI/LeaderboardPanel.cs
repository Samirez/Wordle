using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Wordle.HighScoreStorage;

namespace Wordle.UI
{
    public class LeaderboardPanel : MonoBehaviour
    {
        private const int RankColumnWidth = 6;
        private const int PlayerColumnWidth = 16;
        private const int ScoreColumnWidth = 8;
        private const int TimeColumnWidth = 7;

        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI rowsText;
        [SerializeField, Min(1)] private int maxRows = 10;
        [SerializeField, Min(6)] private int maxPlayerNameLength = 14;
        [SerializeField, Min(10f)] private float rowsMaxFontSize = 26f;
        [SerializeField, Min(8f)] private float rowsMinFontSize = 14f;

        private PlayerRecords playerRecords;

        private void Awake()
        {
            playerRecords = FindFirstObjectByType<PlayerRecords>();

            ResolveHeaderText();

            if (rowsText == null)
            {
                Transform rows = transform.Find("LeaderboardRows");
                if (rows != null)
                {
                    rowsText = rows.GetComponent<TextMeshProUGUI>();
                }
            }

            if (rowsText == null)
            {
                rowsText = CreateRowsTextObject();
            }

            ConfigureHeaderText();
            ConfigureRowsText();
        }

        private void ResolveHeaderText()
        {
            if (headerText != null)
            {
                return;
            }

            Transform header = transform.Find("LeaderboardHeader");
            if (header != null)
            {
                headerText = header.GetComponent<TextMeshProUGUI>();
            }

            if (headerText == null)
            {
                Transform highScoreLabel = transform.Find("HighScoreLabel");
                if (highScoreLabel != null)
                {
                    headerText = highScoreLabel.GetComponent<TextMeshProUGUI>();
                }
            }

            if (headerText == null && transform.parent != null)
            {
                Transform parentHighScoreLabel = transform.parent.Find("HighScoreLabel");
                if (parentHighScoreLabel != null)
                {
                    headerText = parentHighScoreLabel.GetComponent<TextMeshProUGUI>();
                }
            }

            if (headerText == null)
            {
                headerText = CreateHeaderTextObject();
            }
        }

        private TextMeshProUGUI CreateHeaderTextObject()
        {
            GameObject headerObject = new GameObject("LeaderboardHeader");
            RectTransform rect = headerObject.AddComponent<RectTransform>();
            headerObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI text = headerObject.AddComponent<TextMeshProUGUI>();

            rect.SetParent(transform, false);
            rect.anchorMin = new Vector2(0.12f, 0.78f);
            rect.anchorMax = new Vector2(0.88f, 0.95f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            text.alignment = TextAlignmentOptions.TopLeft;

            if (text.font == null && TMP_Settings.defaultFontAsset != null)
            {
                text.font = TMP_Settings.defaultFontAsset;
            }

            return text;
        }

        private void OnEnable()
        {
            EnsureTextVisibility();
            RefreshLeaderboard();
        }

        public void RefreshLeaderboard()
        {
            if (rowsText == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): Rows text is not assigned.");
                return;
            }

            if (headerText != null)
            {
                string headerRow = string.Format(
                    "{0,-" + RankColumnWidth + "}{1,-" + PlayerColumnWidth + "}{2,-" + ScoreColumnWidth + "}{3," + TimeColumnWidth + "}",
                    "RANK",
                    "PLAYER",
                    "SCORE",
                    "TIME");

                headerText.text = WrapMonospace(headerRow);
            }

            if (playerRecords == null)
            {
                playerRecords = FindFirstObjectByType<PlayerRecords>();
                if (playerRecords == null)
                {
                    rowsText.text = "No records found.";
                    Debug.LogWarning($"{name} ({GetType().Name}): PlayerRecords not found.");
                    return;
                }
            }

            List<Record> records = playerRecords.GetAllRecords();
            records.Sort((left, right) =>
            {
                int scoreComparison = right.Score.CompareTo(left.Score);
                if (scoreComparison != 0)
                {
                    return scoreComparison;
                }

                return left.Time.CompareTo(right.Time);
            });

            if (records.Count == 0)
            {
                rowsText.text = "No records yet.";
                return;
            }

            int count = Mathf.Min(maxRows, records.Count);
            rowsText.text = WrapMonospace(BuildRowsText(records, count));
            rowsText.ForceMeshUpdate();

            RectTransform rowsRect = rowsText.rectTransform;
            float availableHeight = rowsRect.rect.height;

            if (rowsText.preferredHeight > availableHeight)
            {
                while (count > 1)
                {
                    count--;
                    rowsText.text = WrapMonospace(BuildRowsText(records, count));
                    rowsText.ForceMeshUpdate();

                    if (rowsText.preferredHeight <= availableHeight)
                    {
                        break;
                    }
                }
            }
        }

        private TextMeshProUGUI CreateRowsTextObject()
        {
            GameObject rowsObject = new GameObject("LeaderboardRows");
            RectTransform rect = rowsObject.AddComponent<RectTransform>();
            rowsObject.AddComponent<CanvasRenderer>();
            TextMeshProUGUI text = rowsObject.AddComponent<TextMeshProUGUI>();

            rect.SetParent(transform, false);
            rect.anchorMin = new Vector2(0.12f, 0.18f);
            rect.anchorMax = new Vector2(0.88f, 0.74f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            text.alignment = TextAlignmentOptions.TopLeft;

            if (text.font == null && TMP_Settings.defaultFontAsset != null)
            {
                text.font = TMP_Settings.defaultFontAsset;
            }

            return text;
        }

        private void ConfigureHeaderText()
        {
            if (headerText == null)
            {
                return;
            }

            headerText.textWrappingMode = TextWrappingModes.NoWrap;
            headerText.overflowMode = TextOverflowModes.Truncate;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = new Color(0.08f, 0.08f, 0.08f, 1f);

            if (headerText.enableAutoSizing)
            {
                headerText.fontSizeMin = Mathf.Max(14f, headerText.fontSizeMin);
                headerText.fontSizeMax = Mathf.Max(headerText.fontSizeMin + 2f, headerText.fontSizeMax);
            }
        }

        private void ConfigureRowsText()
        {
            if (rowsText == null)
            {
                return;
            }

            rowsText.enableAutoSizing = true;
            rowsText.fontSizeMin = rowsMinFontSize;
            rowsText.fontSizeMax = rowsMaxFontSize;
            rowsText.textWrappingMode = TextWrappingModes.NoWrap;
            rowsText.overflowMode = TextOverflowModes.Truncate;
            rowsText.color = new Color(0.08f, 0.08f, 0.08f, 1f);
        }

        private void EnsureTextVisibility()
        {
            if (headerText != null)
            {
                if (headerText.font == null && TMP_Settings.defaultFontAsset != null)
                {
                    headerText.font = TMP_Settings.defaultFontAsset;
                }

                headerText.enabled = true;
                headerText.alpha = 1f;
                headerText.gameObject.SetActive(true);
                headerText.rectTransform.SetAsLastSibling();
            }

            if (rowsText != null)
            {
                if (rowsText.font == null && TMP_Settings.defaultFontAsset != null)
                {
                    rowsText.font = TMP_Settings.defaultFontAsset;
                }

                rowsText.enabled = true;
                rowsText.alpha = 1f;
                rowsText.gameObject.SetActive(true);
                rowsText.rectTransform.SetAsLastSibling();
            }
        }

        private string BuildRowsText(List<Record> records, int count)
        {
            StringBuilder tableBuilder = new StringBuilder();

            for (int index = 0; index < count; index++)
            {
                Record record = records[index];
                string playerName = string.IsNullOrWhiteSpace(record.PlayerName) ? "Unknown" : record.PlayerName;
                if (playerName.Length > maxPlayerNameLength)
                {
                    playerName = playerName.Substring(0, maxPlayerNameLength);
                }

                string timeText = record.Time.ToString("0.0") + "s";
                string row = string.Format(
                    "{0,-" + RankColumnWidth + "}{1,-" + PlayerColumnWidth + "}{2,-" + ScoreColumnWidth + "}{3," + TimeColumnWidth + "}",
                    index + 1,
                    playerName,
                    record.Score,
                    timeText);

                tableBuilder.Append(row);
                if (index < count - 1)
                {
                    tableBuilder.AppendLine();
                }
            }

            return tableBuilder.ToString();
        }

        private static string WrapMonospace(string content)
        {
            return "<mspace=0.62em>" + content + "</mspace>";
        }
    }
}
