using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sphere")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager Instance is NULL!");
            return;
        }

        GameManager.Instance.LevelComplete();
    }

}
