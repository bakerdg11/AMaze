using NUnit.Framework;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "AMaze/Level Database")]
public class LevelDatabase : ScriptableObject
{
    [Header("Levels")]
    public List<LevelData> Levels = new List<LevelData>();

    public LevelData GetLevel(int levelNumber)
    {
        return Levels.Find(level => level != null && level.LevelNumber == levelNumber);
    }

    public int TotalLevels => Levels.Count;

    
}
