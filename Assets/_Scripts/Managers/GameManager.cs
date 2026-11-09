using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LevelData[] levels;
    [SerializeField] private GameObject spherePrefab;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    private GameObject currentLevel;
    private GameObject currentSphere;

    public SphereMovement CurrentSphereMovement { get; private set; }
    public bool levelCompleted { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log($"GameState Changed: {CurrentState}");
    }


    private void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        Debug.Log($"GameState Changed: {CurrentState}");
    }


    public void LoadLevel(int levelNumber)
    {
        LevelData levelData = null;

        foreach (LevelData level in levels)
        {
            if (level != null && level.LevelNumber == levelNumber)
            {
                levelData = level;
                break;
            }
        }

        if (levelData == null || levelData.LevelPrefab == null)
        {
            Debug.LogError($"Level {levelNumber} could not be found!");
            return;
        }

        if (spherePrefab == null)
        {
            Debug.LogError("Sphere Prefab is not assigned!");
            return;
        }

        if (currentLevel != null) Destroy(currentLevel);
        if (currentSphere != null) Destroy(currentSphere);

        currentLevel = Instantiate(levelData.LevelPrefab);

        Level levelComponent = currentLevel.GetComponent<Level>();

        if (levelComponent == null || levelComponent.StartingPoint == null)
        {
            Debug.LogError($"Level {levelNumber} has no StartingPoint assigned!");
            return;
        }

        currentSphere = Instantiate(spherePrefab, levelComponent.StartingPoint.position, Quaternion.identity);

        Rigidbody rb = currentSphere.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CurrentSphereMovement = currentSphere.GetComponent<SphereMovement>();

        levelCompleted = false;

        SetGameState(GameState.Preparation);

        Debug.Log($"Level {levelNumber} Loaded!");
    }



    public void BeginExecution()
    {
        if (CurrentState != GameState.Preparation) return;

        SetGameState(GameState.Execution);
    }



    public void LevelInProgress()
    {

    }

    public void LevelComplete()
    {
        if (CurrentState != GameState.Execution || levelCompleted) return;

        levelCompleted = true;

        SetGameState(GameState.Won);

        Debug.Log("Level Complete!");
    }

    public void CheckLevelResult()
    {
        if (levelCompleted)
        {
            Debug.Log("You win!");
        }
        else
        {
            SetGameState(GameState.Lost);
            Debug.Log("You lost!");
        }
    }

    public void ResetLevel()
    {

    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}