using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MovementInputs : MonoBehaviour
{
    [Header("References")]
    private SphereMovement sphereMovement => GameManager.Instance != null ? GameManager.Instance.CurrentSphereMovement : null;

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

    private bool CanEnterInput()
    {
        return GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Preparation;
    }

    private void OnLeftPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Left Button Pressed");

        movementSequence.Add(MovementDirection.Left);
    }

    private void OnRightPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Right Button Pressed");

        movementSequence.Add(MovementDirection.Right);
    }

    private void OnUpPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Up Button Pressed");

        movementSequence.Add(MovementDirection.Up);
    }

    private void OnDownPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Down Button Pressed");

        movementSequence.Add(MovementDirection.Down);
    }

    private void OnBeginPressed()
    {
        if (!CanEnterInput()) return;

        GameManager.Instance.BeginExecution();

        if (movementSequence.Count > 0)
        {
            sphereMovement.ExecuteMovement(movementSequence);
        }

        movementSequence.Clear();
    }



}
