using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MovementInputs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SphereMovement sphereMovement;

    [Header("Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    [SerializeField] private Button beginButton;

    private List<MovementDirection> movementSequence = new List<MovementDirection>();

    private void Awake()
    {
        leftButton.onClick.AddListener(OnLeftPressed);
        rightButton.onClick.AddListener(OnRightPressed);
        upButton.onClick.AddListener(OnUpPressed);
        downButton.onClick.AddListener(OnDownPressed);
        beginButton.onClick.AddListener(OnBeginPressed);
    }


    private void OnLeftPressed()
    {
        if (sphereMovement.isMoving) return;

        Debug.Log("Left Button Pressed");

        movementSequence.Add(MovementDirection.Left);
    }

    private void OnRightPressed()
    {
        if (sphereMovement.isMoving) return;

        Debug.Log("Right Button Pressed");

        movementSequence.Add(MovementDirection.Right);
    }

    private void OnUpPressed()
    {
        if (sphereMovement.isMoving) return;

        Debug.Log("Up Button Pressed");

        movementSequence.Add(MovementDirection.Up);
    }

    private void OnDownPressed()
    {
        if (sphereMovement.isMoving) return;

        Debug.Log("Down Button Pressed");

        movementSequence.Add(MovementDirection.Down);
    }

    private void OnBeginPressed()
    {
        if (sphereMovement.isMoving) return;

        Debug.Log("Begin Button Pressed");

        sphereMovement.ExecuteMovement(movementSequence);

        movementSequence.Clear();
    }



}
