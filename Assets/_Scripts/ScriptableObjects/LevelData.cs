using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "AMaze/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int LevelNumber;
    public string LevelName;

    [Header("Level Prefab")]
    public GameObject LevelPrefab;

    [Header("Level Settings")]
    public float PreparationTime;
    public int MinimumMovesToComplete;
    
}
