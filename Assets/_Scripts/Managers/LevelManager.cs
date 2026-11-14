using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private GameObject spherePrefab;


    [Header("Level Information")]
    [SerializeField] private LevelData[] levels;
    public IReadOnlyList<LevelData> Levels => levels;
    public LevelData CurrentLevelData { get; private set; }
    public int CurrentLevelNumber { get; private set; } = -1;
    private GameObject currentLevel;
    public int HighestUnlockedLevel { get; private set; } = 1;


    [Header("Sphere Information")]
    public SphereMovement CurrentSphereMovement { get; private set; }
    private GameObject currentSphere;


    [Header("Testing")]
    [SerializeField] private bool testMode = false;


    public bool LoadLevel(int levelNumber)
    {
        if (!IsLevelUnlocked(levelNumber))
        {
            Debug.LogWarning($"Level {levelNumber} is locked!");
            return false;
        }

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
            return false;
        }

        if (spherePrefab == null)
        {
            Debug.LogError("Sphere Prefab is not assigned!");
            return false;
        }

        if (cameraManager == null)
        {
            Debug.LogError("CameraManager is not assigned!");
            return false;
        }

        Level levelComponent = levelData.LevelPrefab.GetComponent<Level>();

        if (levelComponent == null || levelComponent.StartingPoint == null)
        {
            Debug.LogError($"Level {levelNumber} has no StartingPoint assigned!");
            return false;
        }

        if (currentLevel != null) Destroy(currentLevel);
        if (currentSphere != null) Destroy(currentSphere);

        currentLevel = Instantiate(levelData.LevelPrefab);

        levelComponent = currentLevel.GetComponent<Level>();

        currentSphere = Instantiate(spherePrefab, levelComponent.StartingPoint.position, Quaternion.identity);

        Rigidbody rb = currentSphere.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CurrentSphereMovement = currentSphere.GetComponent<SphereMovement>();

        CurrentLevelNumber = levelNumber;
        CurrentLevelData = levelData;

        cameraManager.AdjustCameraToLevel(currentLevel);

        Debug.Log($"Level {levelNumber} Loaded!");

        return true;
    }

    public bool ResetLevel()
    {
        if (CurrentLevelNumber < 0)
        {
            Debug.LogWarning("No level is currently loaded");
            return false;
        }

        return LoadLevel(CurrentLevelNumber);
    }

    public bool LoadNextLevel()
    {
        int nextLevelNumber = CurrentLevelNumber + 1;

        foreach (LevelData level in levels)
        {
            if (level == null || level.LevelNumber != nextLevelNumber) continue;

            return LoadLevel(nextLevelNumber);
        }

        Debug.LogWarning($"Level {nextLevelNumber} does not exist!");
        return false;
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        foreach (LevelData level in levels)
        {
            if (level == null || level.LevelNumber != levelNumber) continue;

            return testMode || levelNumber <= HighestUnlockedLevel;
        }

        return false;
    }

    public void UnlockNextLevel(int completedLevelNumber)
    {
        int nextLevelNumber = completedLevelNumber + 1;

        foreach (LevelData level in levels)
        {
            if (level == null || level.LevelNumber != nextLevelNumber) continue;

            if (nextLevelNumber > HighestUnlockedLevel)
            {
                HighestUnlockedLevel = nextLevelNumber;
            }

            return;
        }

    }




}