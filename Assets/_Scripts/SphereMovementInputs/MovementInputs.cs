using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MovementInputs : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    [SerializeField] private Button beginButton;
    [SerializeField] private SphereMovement sphereMovement;

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
        Debug.Log("Left Button Pressed");

        if (sphereMovement.isMoving) return;
        movementSequence.Add(MovementDirection.Left);
    }

    private void OnRightPressed()
    {
        Debug.Log("Right Button Pressed");

        if (sphereMovement.isMoving) return;
        movementSequence.Add(MovementDirection.Right);
    }

    private void OnUpPressed()
    {
        Debug.Log("Up Button Pressed");

        if (sphereMovement.isMoving) return;
        movementSequence.Add(MovementDirection.Up);
    }

    private void OnDownPressed()
    {
        Debug.Log("Down Button Pressed");

        if (sphereMovement.isMoving) return;
        movementSequence.Add(MovementDirection.Down);
    }

    private void OnBeginPressed()
    {
        Debug.Log("Begin Button Pressed");

        if (sphereMovement.isMoving) return;
        sphereMovement.ExecuteMovement(movementSequence);
    }











}
