using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField, UnityEngine.Range(30f, 90f)] private float cameraAngle = 60f;
    [SerializeField, UnityEngine.Range(20f, 80f)] private float cameraFieldOfView = 45f;
    [SerializeField, UnityEngine.Range(0f, 0.4f)] private float verticalScreenPadding = 0.15f;
    [SerializeField, UnityEngine.Range(0f, 0.4f)] private float horizontalScreenPadding = 0.05f;

    [SerializeField] private MovementInputs movementInputs;

    [SerializeField] private LevelData[] levels;
    [SerializeField] private GameObject spherePrefab;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public event Action<GameState> OnGameStateChanged;
    public int CurrentLevelNumber { get; private set; } = -1;
    public LevelData CurrentLevelData { get; private set; }

    public int InputsUsed { get; private set; }

    private GameObject currentLevel;
    private GameObject currentSphere;

    [Header("Preparation Timer")]
    [SerializeField] private TMP_Text countdownText;
    private float remainingTime;
    private Coroutine countdownCoroutine;

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

        if (newState == GameState.Won || newState == GameState.Lost)
        {
            movementInputs.ResetInputs();
        }

        OnGameStateChanged?.Invoke(CurrentState);
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

        CurrentLevelNumber = levelNumber;
        CurrentLevelData = levelData;

        AdjustCameraToLevel();
        StartPreparation();

        Debug.Log($"Level {levelNumber} Loaded!");
    }


    private void AdjustCameraToLevel()
    {
        if (mainCamera == null || currentLevel == null || CurrentLevelData == null) return;

        Renderer[] renderers = currentLevel.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found on the current level!");
            return;
        }

        Bounds levelBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            levelBounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 center = levelBounds.center;

        // Set up perspective camera
        mainCamera.orthographic = false;
        mainCamera.fieldOfView = cameraFieldOfView;
        mainCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        // Calculate usable screen space
        float usableHeight = 1f - (verticalScreenPadding * 2f);
        float usableWidth = 1f - (horizontalScreenPadding * 2f);

        // Calculate camera field of view
        float verticalFOV = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV / 2f) * mainCamera.aspect);

        // Calculate the distance required to fit the entire level
        float requiredDistance = 0f;

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 corner = center + Vector3.Scale(levelBounds.extents, new Vector3(x, y, z));

                    Vector3 relativeCorner = corner - center;

                    Vector3 cameraSpaceCorner = Quaternion.Inverse(mainCamera.transform.rotation) * relativeCorner;

                    float verticalDistance = Mathf.Abs(cameraSpaceCorner.y) / (Mathf.Tan(verticalFOV / 2f) * usableHeight);
                    float horizontalDistance = Mathf.Abs(cameraSpaceCorner.x) / (Mathf.Tan(horizontalFOV / 2f) * usableWidth);

                    float cornerDistance = Mathf.Max(verticalDistance, horizontalDistance) + cameraSpaceCorner.z;

                    requiredDistance = Mathf.Max(requiredDistance, cornerDistance);
                }
            }
        }

        // Position camera so the entire maze is visible
        requiredDistance = Mathf.Max(requiredDistance, mainCamera.nearClipPlane + levelBounds.extents.magnitude + 0.1f);

        mainCamera.transform.position = center - mainCamera.transform.forward * requiredDistance;

        Debug.Log($"Camera adjusted for Level {CurrentLevelNumber}. Distance: {requiredDistance}");
    }

    private void StartPreparation()
    {
        if (CurrentLevelData == null)
        {
            Debug.LogError("CurrentLevelData is null");
            return;
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        SetGameState(GameState.Preparation);

        remainingTime = CurrentLevelData.PreparationTime;

        countdownText.gameObject.SetActive(true);
        countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

        countdownCoroutine = StartCoroutine(PreparationCountdown());

    }

    private IEnumerator PreparationCountdown()
    {
        while (remainingTime > 0f && CurrentState == GameState.Preparation)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(remainingTime, 0f);

            countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

            yield return null;
        }

        countdownCoroutine = null;

        if (CurrentState == GameState.Preparation)
        {
            BeginExecution();
        }
    }



    public void BeginExecution()
    {
        if (CurrentState != GameState.Preparation) return;

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        countdownText.gameObject.SetActive(false);

        SetGameState(GameState.Execution);

        if (CurrentSphereMovement == null || movementInputs == null)
        {
            Debug.LogError("SphereMovement or MovementInputs reference is missing!");
            CheckLevelResult();
            return;
        }

        List<MovementDirection> sequence = movementInputs.GetMovementSequence();

        // Sets InputsUsed as the amount of inputs pressed to check record
        InputsUsed = sequence.Count;

        if (sequence.Count == 0)
        {
            Debug.Log("No movement sequence entered");
            CheckLevelResult();
            return;
        }

        CurrentSphereMovement.ExecuteMovement(sequence);

        movementInputs.ClearStoredInputs();

        Debug.Log("Execution phase started");


    }


    public void HighlightCurrentMovement(int index)
    {
        if (movementInputs == null) return;

        movementInputs.HighlightMovement(index);
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
        if (CurrentState != GameState.Execution) return;

        SetGameState(GameState.Lost);

        Debug.Log("You lost!");
    }

    public void ResetLevel()
    {
        if (CurrentLevelNumber < 0)
        {
            Debug.LogWarning("No level is currently loaded");
            return;
        }

        movementInputs.ResetInputs();

        LoadLevel(CurrentLevelNumber);
    }

    public void LoadNextLevel()
    {
        int nextLevelNumber = CurrentLevelNumber + 1;

        foreach (LevelData level in levels)
        {
            if (level == null || level.LevelNumber != nextLevelNumber) continue;

            LoadLevel(nextLevelNumber);
            return;
        }
    }


    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}