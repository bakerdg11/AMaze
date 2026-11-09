using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startingPoint;

    public Transform StartingPoint => startingPoint;
}
