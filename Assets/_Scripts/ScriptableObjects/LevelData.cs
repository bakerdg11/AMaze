using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "AMaze/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int LevelNumber;
    public string LevelName;

    [Header("Level Prefab")]
    public GameObject LevelPrefab;

    [Header("Camera Settings")]
    public Vector3 CameraPosition = new Vector3(0f, 15f, 0f);
    public float CameraOrthographicSize = 10f;

    [Header("Level Settings")]
    public int MaxMovementCommands;
    public int MaxObjectsToPlace;
    
}
