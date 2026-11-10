using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MovementInputs movementInputs;

    [SerializeField] private LevelData[] levels;
    [SerializeField] private GameObject spherePrefab;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public event Action<GameState> OnGameStateChanged;
    public int CurrentLevelNumber { get; private set; } = -1;

    private GameObject currentLevel;
    private GameObject currentSphere;

    [Header("Preparation Timer")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float preparationTime = 7f;

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

        StartPreparation();

        Debug.Log($"Level {levelNumber} Loaded!");
    }



    private void StartPreparation()
    {
        SetGameState(GameState.Preparation);

        remainingTime = preparationTime;

        countdownText.gameObject.SetActive(true);
        countdownText.text = Mathf.CeilToInt(remainingTime).ToString();

        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

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

        if (sequence.Count == 0)
        {
            Debug.Log("No movement sequence entered");
            CheckLevelResult();
            return;
        }

        CurrentSphereMovement.ExecuteMovement(sequence);

        movementInputs.ResetInputs();

        Debug.Log("Execution phase started");


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
        if (CurrentLevelNumber < 0)
        {
            Debug.LogWarning("No level is currently loaded");
            return;
        }

        movementInputs.ResetInputs();

        LoadLevel(CurrentLevelNumber);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}