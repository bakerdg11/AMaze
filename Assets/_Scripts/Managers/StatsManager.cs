using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [Header("Current Run Stats")]
    public int InputsUsed {  get; private set; }

    private Dictionary<int, int> levelRecords = new Dictionary<int, int>();

    public void SetInputsUsed(int count)
    {
        InputsUsed = count;
    }

    public void RecordLevelCompletion(int levelNumber)
    {
        if (!levelRecords.ContainsKey(levelNumber) || InputsUsed < levelRecords[levelNumber])
        {
            levelRecords[levelNumber] = InputsUsed;

            Debug.Log($"New record for Level {levelNumber}: {InputsUsed}");
        }
    }

    public int GetLevelRecord(int levelNumber)
    {
        if (levelRecords.TryGetValue(levelNumber, out int record))
        {
            return record;
        }

        return 0;
    }

    public void ResetCurrentRun()
    {
        InputsUsed = 0;
    }



}
