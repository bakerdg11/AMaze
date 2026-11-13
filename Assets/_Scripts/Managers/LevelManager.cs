using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private LevelData[] levels;
    [SerializeField] private GameObject spherePrefab;
    public IReadOnlyList<LevelData> Levels => levels;

    [Header("Manager References")]
    [SerializeField] private CameraManager cameraManager;

    public int CurrentLevelNumber { get; private set; } = -1;
    public LevelData CurrentLevelData { get; private set; }
    public SphereMovement CurrentSphereMovement { get; private set; }

    private GameObject currentLevel;
    private GameObject currentSphere;

    public bool LoadLevel(int levelNumber)
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
}