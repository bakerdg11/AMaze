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

    [Header("Arrow Display UI")]
    [SerializeField] private MovementSequenceUI movementSequenceUI;

    private List<MovementDirection> movementSequence = new List<MovementDirection>();

    private void Awake()
    {
        leftButton.onClick.AddListener(OnLeftPressed);
        rightButton.onClick.AddListener(OnRightPressed);
        upButton.onClick.AddListener(OnUpPressed);
        downButton.onClick.AddListener(OnDownPressed);
        beginButton.onClick.AddListener(OnBeginPressed);
    }

    public List<MovementDirection> GetMovementSequence()
    {
        return new List<MovementDirection>(movementSequence);
    }

    private bool CanEnterInput()
    {
        return GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Preparation;
    }

    public void ResetInputs()
    {
        movementSequence.Clear();
        movementSequenceUI.ClearSequence();
    }

    public void ClearStoredInputs()
    {
        movementSequence.Clear();
    }

    private void OnLeftPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Left Button Pressed");

        movementSequence.Add(MovementDirection.Left);
        movementSequenceUI.AddMovement(MovementDirection.Left);
    }

    private void OnRightPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Right Button Pressed");

        movementSequence.Add(MovementDirection.Right);
        movementSequenceUI.AddMovement(MovementDirection.Right);
    }

    private void OnUpPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Up Button Pressed");

        movementSequence.Add(MovementDirection.Up);
        movementSequenceUI.AddMovement(MovementDirection.Up);
    }

    private void OnDownPressed()
    {
        if (!CanEnterInput()) return;

        Debug.Log("Down Button Pressed");

        movementSequence.Add(MovementDirection.Down);
        movementSequenceUI.AddMovement(MovementDirection.Down);
    }

    private void OnBeginPressed()
    {
        if (!CanEnterInput()) return;

        GameManager.Instance.BeginExecution();
    }


    public void HighlightMovement(int index)
    {
        movementSequenceUI.HighlightMovement(index);
    }




}
