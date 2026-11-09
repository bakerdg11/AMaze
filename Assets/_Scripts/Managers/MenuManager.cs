using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject hudPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button selectLevelButton;
    [SerializeField] private Button quitButton;

    [Header("Level Select Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button level1Button;

    private void Awake()
    {
        selectLevelButton.onClick.AddListener(OpenLevelSelect);
        quitButton.onClick.AddListener(QuitGame);

        backButton.onClick.AddListener(CloseLevelSelect);
        level1Button.onClick.AddListener(PlayLevel1);
    }


    private void Start()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        hudPanel.SetActive(false);
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
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        hudPanel.SetActive(true);

        GameManager.Instance.LoadLevel(1);
    }

    private void QuitGame()
    {
        Application.Quit();
    }





}
