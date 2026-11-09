using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]

public class SphereMovement : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider sphereCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Movement Bools")]
    public bool isMoving { get; private set; } = false;

    [Header("Wall Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallStopDistance = 0.5f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void ExecuteMovement(List<MovementDirection> movementSequence)
    {
        if (isMoving || movementSequence == null || movementSequence.Count == 0) return;

        isMoving = true;
        Debug.Log("IsMoving = true");

        StartCoroutine(MoveSequence(new List<MovementDirection>(movementSequence)));
    }

    private IEnumerator MoveSequence(List<MovementDirection> movementSequence)
    {
        foreach (MovementDirection direction in movementSequence)
        {
            Vector3 moveDirection = GetDirection(direction);

            while (!CheckForWall(moveDirection))
            {
                rb.linearVelocity = moveDirection * moveSpeed;
                yield return new WaitForFixedUpdate();
            }

            rb.linearVelocity = Vector3.zero;

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;

        isMoving = false;
        Debug.Log("IsMoving = false");

        GameManager.Instance.CheckLevelResult();
    }


    private bool CheckForWall(Vector3 direction)
    {
        Vector3 origin = transform.TransformPoint(sphereCollider.center);

        float radius = sphereCollider.radius * Mathf.Max(
            Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y),
            Mathf.Abs(transform.lossyScale.z)
        );

        float checkDistance = moveSpeed * Time.fixedDeltaTime + wallStopDistance;

        return Physics.SphereCast(origin, radius, direction, out RaycastHit hit, checkDistance, wallLayer, QueryTriggerInteraction.Ignore);
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
                return Vector3.back;

            default:
                return Vector3.zero;
        }
    }


}
