using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelText;

    private int levelNumber;
    private Action<int> onLevelSelected;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonPressed);
    }

    public void Setup(int number, Action<int> callback)
    {
        levelNumber = number;
        onLevelSelected = callback;

        levelText.text = $"Level {levelNumber}";
    }

    private void OnButtonPressed()
    {
        onLevelSelected?.Invoke(levelNumber);
    }

}
