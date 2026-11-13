using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    [Header("Managers")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private MovementInputs movementInputs;
    [SerializeField] private StatsManager statsManager;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public event Action<GameState> OnGameStateChanged;

    public int CurrentLevelNumber => levelManager.CurrentLevelNumber;
    public LevelData CurrentLevelData => levelManager.CurrentLevelData;
    public SphereMovement CurrentSphereMovement => levelManager.CurrentSphereMovement;

    [Header("Preparation Timer")]
    [SerializeField] private TMP_Text countdownText;
    private float remainingTime;
    private Coroutine countdownCoroutine;

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
        if (levelManager == null)
        {
            Debug.LogError("LevelManager is not assigned!");
            return;
        }

        if (!levelManager.LoadLevel(levelNumber)) return;

        movementInputs.ResetInputs();

        levelCompleted = false;
        statsManager.ResetCurrentRun();

        StartPreparation();
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
        statsManager.SetInputsUsed(sequence.Count);

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

        statsManager.RecordLevelCompletion(CurrentLevelNumber);

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

        LoadLevel(CurrentLevelNumber);
    }

    public void LoadNextLevel()
    {
        int nextLevelNumber = CurrentLevelNumber + 1;

        LoadLevel(nextLevelNumber);
    }


    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}