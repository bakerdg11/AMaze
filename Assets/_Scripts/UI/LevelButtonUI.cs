using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class LevelButtonUI : MonoBehaviour
{
    [Header("References")]
    private LevelData levelData;
    private StatsManager statsManager;

    [Header("Buttons and Text")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelRecordText;

    [Header("Record Colous")]
    [SerializeField] private Color incompleteColor = Color.white;
    [SerializeField] private Color incompleteRecordColor = Color.red;
    [SerializeField] private Color perfectRecordColor = Color.green;

    private Action<int> onLevelSelected;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonPressed);
    }

    public void Setup(LevelData data, StatsManager stats, Action<int> callback)
    {
        levelData = data;
        statsManager = stats;
        onLevelSelected = callback;

        levelText.text = $"Level {levelData.LevelNumber}";

        RefreshRecord();
    }

    public void RefreshRecord()
    {
        if (levelData == null || statsManager == null) return;

        int record = statsManager.GetLevelRecord(levelData.LevelNumber);

        if (record == 0)
        {
            levelRecordText.text = "Record: -";
            levelRecordText.color = incompleteColor;
        }
        else if (record == levelData.MinimumMovesToComplete)
        {
            levelRecordText.text = $"RECORD: {record}";
            levelRecordText.color = perfectRecordColor;
        }
        else
        {
            levelRecordText.text = $"RECORD: {record}";
            levelRecordText.color = incompleteRecordColor;
        }
    }

    private void OnButtonPressed()
    {
        if (levelData == null) return;

        onLevelSelected?.Invoke(levelData.LevelNumber);
    }

}
