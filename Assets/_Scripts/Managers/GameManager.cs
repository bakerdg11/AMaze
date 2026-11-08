using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //[SerializeField] private SphereMovement sphereMovement;

    private bool levelCompleted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LevelComplete()
    {
        if (levelCompleted) return;

        levelCompleted = true;

        Debug.Log("Level Complete!");
    }



}
