using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private Vector2[] pathingPoints;
    [SerializeField] private OverworldMovement overworldMovement;
    public Vector2 movementTarget = Vector2.zero;
    private int currentPointIndex = 0;
    [SerializeField] private LayerMask mask;
    public bool chasingPlayer;
    private int playerChaseCount = 0;
    private Transform playerPos;
    private void Start()
    {
        overworldMovement.SetUnit(GetComponent<OverworldEnemyObject>().GetEnemies()[0].unit);
        SetMovementTarget();
    }
    private void Update()
    {
        if (overworldMovement.moving)
            return;
        else
        {
            MoveToTarget();
        }
    }
    private void SetMovementTarget()
    {
        Vector2 closestPoint = pathingPoints[0];
        currentPointIndex = 0;
        int currentPoint = 0;
        foreach (Vector2 point in pathingPoints)
        {
            if (Vector3.Distance(transform.position, point) < Vector3.Distance(transform.position, closestPoint))
            {
                closestPoint = point;
                currentPointIndex = currentPoint;
            }
            currentPoint++;
        }
        movementTarget = closestPoint;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        
        if (Mathf.FloorToInt(movementTarget.y) > Mathf.FloorToInt(transform.position.y))
            overworldMovement.MovePos(new Vector2(0, 1));
        if (Mathf.FloorToInt(movementTarget.x) < Mathf.FloorToInt(transform.position.x))
            overworldMovement.MovePos(new Vector2(-1, 0));
        else if (Mathf.FloorToInt(movementTarget.x) > Mathf.FloorToInt(transform.position.x))
            overworldMovement.MovePos(new Vector2(1, 0));
        else if(Mathf.FloorToInt(movementTarget.y) < Mathf.FloorToInt(transform.position.y))
            overworldMovement.MovePos(new Vector2(0, -1));

        if (chasingPlayer)
        {
            playerChaseCount--;
            if (playerChaseCount <= 0)
            {
                chasingPlayer = false;
                SetMovementTarget();
            }
            else
            {
                movementTarget = playerPos.position;
            }
        }
        SendVision();
        if (Vector3.Distance(transform.position, movementTarget) < 0.001f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathingPoints.Length)
                currentPointIndex = 0;
            movementTarget = pathingPoints[currentPointIndex];
        }
    }

    private void SendVision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, overworldMovement.GetFacingDir(), 3.25f, mask);
        if (hit)
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                playerPos = hit.collider.gameObject.transform;
                chasingPlayer = true;
                playerChaseCount = 8;
                movementTarget = playerPos.position;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathingPoints.Length <= 0)
            return;
        Gizmos.color = Color.red;
        float gizSize = 0.1f;
        foreach (Vector2 point in pathingPoints)
        {
            Gizmos.DrawSphere(point, gizSize);
            gizSize += 0.025f;
        }
    }
}
