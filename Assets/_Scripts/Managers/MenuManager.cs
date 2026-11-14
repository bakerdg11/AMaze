using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Refernces")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private StatsManager statsManager;

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Main Menu")]
    [SerializeField] private Button selectLevelButton;
    [SerializeField] private Button quitButton;

    [Header("Level Select")]
    [SerializeField] private Button backButton;
    [SerializeField] private LevelButtonUI levelButtonPrefab;
    [SerializeField] private Transform levelButtonContainer;

    [Header("Win Panel")]
    [SerializeField] private Button winTryAgainButton;
    [SerializeField] private Button winNextLevelButton;
    [SerializeField] private Button winSelectLevelButton;
    [SerializeField] private TMP_Text textInputsNumber;
    [SerializeField] private TMP_Text textInputsNumberRecord;

    [Header("Lose Panel")]
    [SerializeField] private Button loseTryAgainButton;
    [SerializeField] private Button loseSelectLevelButton;

    private void Awake()
    {
        //Main Menu Buttons
        selectLevelButton.onClick.AddListener(OpenLevelSelect);
        quitButton.onClick.AddListener(QuitGame);

        // Level Select Buttons
        backButton.onClick.AddListener(CloseLevelSelect);

        // Win Panel Buttons
        winTryAgainButton.onClick.AddListener(WinTryAgain);
        winNextLevelButton.onClick.AddListener(WinNextLevel);
        winSelectLevelButton.onClick.AddListener(WinSelectLevel);

        // Lose Panel Buttons
        loseTryAgainButton.onClick.AddListener(LoseTryAgain);
        loseSelectLevelButton.onClick.AddListener(LoseSelectLevel);
    }


    private void Start()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        hudPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        GenerateLevelButtons();

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void GenerateLevelButtons()
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (LevelData level in levelManager.Levels)
        {
            if (level == null) continue;

            LevelButtonUI newButton = Instantiate(levelButtonPrefab, levelButtonContainer);

            newButton.Setup(level, statsManager, levelManager, PlayLevel);
        }
    }

    private void RefreshLevelButtons()
    {
        LevelButtonUI[] levelButtons = levelButtonContainer.GetComponentsInChildren<LevelButtonUI>(true);
        
        foreach (LevelButtonUI levelButton in levelButtons)
        {
            levelButton.RefreshRecord();
            levelButton.RefreshUnlockStatus();
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Won)
        {
            UpdateWinPanelStats();
        }

        winPanel.SetActive(newState == GameState.Won);
        losePanel.SetActive(newState == GameState.Lost);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void BeginGame()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        hudPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private void OpenLevelSelect()
    {
        RefreshLevelButtons();


        levelSelectPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void PlayLevel(int levelNumber)
    {
        GameManager.Instance.LoadLevel(levelNumber);
        BeginGame();
    }

    private void UpdateWinPanelStats()
    {
        textInputsNumber.text = statsManager.InputsUsed.ToString();
        textInputsNumberRecord.text = statsManager.GetLevelRecord(levelManager.CurrentLevelNumber).ToString();
    }

    private void WinNextLevel()
    {
        if (GameManager.Instance == null) return;

        int previousLevel = GameManager.Instance.CurrentLevelNumber;

        GameManager.Instance.LoadNextLevel();

        if (GameManager.Instance.CurrentLevelNumber != previousLevel)
        {
            BeginGame();
        }
    }

    private void WinTryAgain()
    {
        GameManager.Instance.ResetLevel();
        BeginGame();
    }

    private void WinSelectLevel()
    {
        RefreshLevelButtons();

        hudPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        winPanel.SetActive(false);
    }

    private void LoseTryAgain()
    {
        GameManager.Instance.ResetLevel();
        BeginGame();
    }

    private void LoseSelectLevel()
    {
        RefreshLevelButtons();

        hudPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        losePanel.SetActive(false);
    }



    private void QuitGame()
    {
        Application.Quit();
    }
}
