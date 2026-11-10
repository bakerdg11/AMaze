using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button selectLevelButton;
    [SerializeField] private Button quitButton;

    [Header("Level Select Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button level1Button;

    [Header("Win Panel Buttons")]
    [SerializeField] private Button winTryAgainButton;
    [SerializeField] private Button winNextLevelButton;
    [SerializeField] private Button winSelectLevelButton;

    [Header("Lose Panel Buttons")]
    [SerializeField] private Button loseTryAgainButton;
    [SerializeField] private Button loseSelectLevelButton;

    private void Awake()
    {
        //Main Menu Buttons
        selectLevelButton.onClick.AddListener(OpenLevelSelect);
        quitButton.onClick.AddListener(QuitGame);

        // Level Select Buttons
        backButton.onClick.AddListener(CloseLevelSelect);
        level1Button.onClick.AddListener(PlayLevel1);

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

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void HandleGameStateChanged(GameState newState)
    {
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
        levelSelectPanel.SetActive(true);
    }

    private void CloseLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    private void PlayLevel1()
    {
        BeginGame();

        GameManager.Instance.LoadLevel(1);
    }

    private void WinNextLevel()
    {
        Debug.Log("Move to next level");
    }

    private void WinTryAgain()
    {
        BeginGame();
        GameManager.Instance.ResetLevel();
    }

    private void WinSelectLevel()
    {
        levelSelectPanel.SetActive(true);
        winPanel.SetActive(false);
    }

    private void LoseTryAgain()
    {
        BeginGame();
        GameManager.Instance.ResetLevel();
    }

    private void LoseSelectLevel()
    {
        levelSelectPanel.SetActive(true);
        losePanel.SetActive(false);
    }



    private void QuitGame()
    {
        Application.Quit();
    }
}
