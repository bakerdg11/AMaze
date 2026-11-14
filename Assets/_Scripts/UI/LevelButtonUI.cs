using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class LevelButtonUI : MonoBehaviour
{
    [Header("References")]
    private LevelData levelData;
    private LevelManager levelManager;
    private StatsManager statsManager;

    [Header("Buttons, Texts and Images")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text levelRecordText;
    [SerializeField] private GameObject lockIcon;

    [Header("Level Seelct Colours")]
    [SerializeField] private Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;
    [SerializeField] private Color incompleteColor = Color.white;
    [SerializeField] private Color incompleteRecordColor = Color.red;
    [SerializeField] private Color perfectRecordColor = Color.green;

    private Action<int> onLevelSelected;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonPressed);
    }

    public void Setup(LevelData data, StatsManager stats, LevelManager manager, Action<int> callback)
    {
        levelData = data;
        statsManager = stats;
        levelManager = manager;
        onLevelSelected = callback;

        levelText.text = $"Level {levelData.LevelNumber}";

        RefreshRecord();
        RefreshUnlockStatus();
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


    public void RefreshUnlockStatus()
    {
        if (levelData == null || levelManager == null) return;

        bool isUnlocked = levelManager.IsLevelUnlocked(levelData.LevelNumber);

        button.interactable = isUnlocked;

        levelText.color = isUnlocked ? unlockedTextColor : lockedTextColor;

        lockIcon.SetActive(!isUnlocked);

        levelText.gameObject.SetActive(true);

        levelRecordText.gameObject.SetActive(isUnlocked);
    }


    private void OnButtonPressed()
    {
        if (levelData == null || levelManager == null) return;

        if (!levelManager.IsLevelUnlocked(levelData.LevelNumber)) return;

        onLevelSelected?.Invoke(levelData.LevelNumber);
    }

}
