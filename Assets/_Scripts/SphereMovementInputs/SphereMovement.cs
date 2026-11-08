using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class SphereMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private bool hitWall;
    public bool isMoving { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ExecuteMovement(List<MovementDirection> movementSequence)
    {
        if (isMoving || movementSequence == null || movementSequence.Count == 0) return;

        StartCoroutine(MoveSequence(new List<MovementDirection>(movementSequence)));
    }

    private IEnumerator MoveSequence(List<MovementDirection> movementSequence)
    {
        isMoving = true;
        Debug.Log("IsMoving = true");

        foreach (MovementDirection direction in movementSequence)
        {
            hitWall = false;

            Vector3 moveDirection = GetDirection(direction);

            while (!hitWall)
            {
                rb.linearVelocity = moveDirection * moveSpeed;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }

        isMoving = false;
        Debug.Log("IsMoving = false");
        Debug.Log("Movement sequence completed");
    }

    private Vector3 GetDirection(MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.Left:
                return Vector3.left;

            case MovementDirection.Right:
                return Vector3.right;

            case MovementDirection.Up:
                return Vector3.forward;

            case MovementDirection.Down:
                return Vector3.down;

            default:
                return Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            hitWall = true;
        }
    }







}
