using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] private bool randomMovement;
    [SerializeField] private int randomMovementRange; //0 == CompletelyRandom
    [SerializeField] private Vector2[] pathingPoints;
    [SerializeField] private OverworldMovement overworldMovement;
    public Vector2 movementTarget = Vector2.zero;
    private int currentPointIndex = 0;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float sightRange = 3.25f;
    public bool chasingPlayer;
    private int playerChaseCount = 0;
    private int tilesTillStopChase = 8;
    private Transform playerPos;
    private bool idle;
    [SerializeField] private float waitTime;
    [SerializeField] private bool tall;
    [SerializeField] private GameObject alertSprite;
    private void Start()
    {
        startPos = transform.position;
        overworldMovement.SetUnit(GetComponent<OverworldEnemyObject>().GetEnemies()[0].unit);
        if (tall)
            alertSprite.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        alertSprite.SetActive(false);
        SetMovementTarget();
    }
    private void Update()
    {
        SendVision();
        if (overworldMovement.moving || idle)
            return;
        else
        {
            MoveToTarget();
        }
    }
    private void SetMovementTarget()
    {
        if (!randomMovement)
        {
            Vector2 closestPoint = pathingPoints[0];//Pathing Using Points
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
        }
        else
        {
            SetRandomMovementTarget();
        }

        MoveToTarget();
    }

    private void SetRandomMovementTarget()
    {
        int moveX = Random.Range(0, 2);
        int moveDir = Random.Range(-1, 2);
        if (moveX > 0)
        {
            if (randomMovementRange != 0 && transform.position.x + moveDir > startPos.x + (randomMovementRange * 0.5f) || transform.position.x + moveDir < startPos.x - (randomMovementRange * 0.5f))
            {
                movementTarget = transform.position + new Vector3(-moveDir, 0, 0);
            }
            else
                movementTarget = transform.position + new Vector3(moveDir, 0, 0);
        }
        else
        {
            if (randomMovementRange != 0 && transform.position.y + moveDir > startPos.y + (randomMovementRange * 0.5f) || transform.position.y + moveDir < startPos.y - (randomMovementRange * 0.5f))
            {
                movementTarget = transform.position + new Vector3(0, -moveDir, 0);
            }
            else
                movementTarget = transform.position + new Vector3(0, moveDir, 0);
        }
    }

    private void MoveToTarget()
    {
        if (Mathf.FloorToInt(movementTarget.y) > Mathf.FloorToInt(transform.position.y))
            overworldMovement.MovePos(new Vector2(0, 1), chasingPlayer);
        if (Mathf.FloorToInt(movementTarget.x) < Mathf.FloorToInt(transform.position.x))
            overworldMovement.MovePos(new Vector2(-1, 0), chasingPlayer);
        else if (Mathf.FloorToInt(movementTarget.x) > Mathf.FloorToInt(transform.position.x))
            overworldMovement.MovePos(new Vector2(1, 0), chasingPlayer);
        else if(Mathf.FloorToInt(movementTarget.y) < Mathf.FloorToInt(transform.position.y))
            overworldMovement.MovePos(new Vector2(0, -1), chasingPlayer);
        if (chasingPlayer)
        {
            playerChaseCount--;
            if (playerChaseCount <= 0)
            {
                alertSprite.SetActive(false);
                chasingPlayer = false;
                SetMovementTarget();
            }
            else
            {
                movementTarget = playerPos.position;
            }
        }
        
        if (Vector3.Distance(transform.position, movementTarget) < 0.001f)
        {
            ReachedTarget();
        }
        else if (WallCheck())
        {
            SetRandomMovementTarget();
        }
    }

    private void ReachedTarget()
    {
        SendVision();
        if (!randomMovement)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathingPoints.Length)
                currentPointIndex = 0;
            movementTarget = pathingPoints[currentPointIndex];
        }
        else
        {
            SetRandomMovementTarget();
        }
        
        if (!chasingPlayer && waitTime > 0)
        {
            StartCoroutine(WaitAfterStop(waitTime));
        }
    }

    private IEnumerator WaitAfterStop(float timer)
    {
        idle = true;
        yield return new WaitForSeconds(timer);
        idle = false;
        SendVision();
    }

    private void SendVision()
    {
        if (!WallCheck())
        {
            Vector2 facingDir = overworldMovement.GetFacingDir();
            for (int i = -1; i <= 1; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + (facingDir.y * i), transform.position.y + (facingDir.x * i)), facingDir, sightRange, mask);
                Debug.DrawLine(new Vector2(transform.position.x + (facingDir.y * i), transform.position.y + (facingDir.x * i)), (new Vector2(transform.position.x + (facingDir.y * i), transform.position.y + (facingDir.x * i)) + (facingDir * sightRange)));
                if (hit)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        playerPos = hit.collider.gameObject.transform;
                        chasingPlayer = true;
                        alertSprite.SetActive(true);
                        StartCoroutine(WaitAfterStop(waitTime * 0.5f));
                        playerChaseCount = tilesTillStopChase;
                        movementTarget = playerPos.position;
                        idle = false;
                    }
                }
            }
        }
    }

    private bool WallCheck()
    {
        Vector2 facingDir = overworldMovement.GetFacingDir();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDir, 0.6f, mask);
        if (hit)
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (randomMovementRange > 0)
        {
            Gizmos.DrawWireSphere(transform.position, randomMovementRange);
        }
        
        if (pathingPoints.Length <= 0)
            return;
        if (!randomMovement)
        {
            Gizmos.color = Color.red;
            float gizSize = 0.1f;
            foreach (Vector2 point in pathingPoints)
            {
                Gizmos.DrawSphere(point, gizSize);
                gizSize += 0.025f;
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(movementTarget, 0.15f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            idle = true;
        }
    }
}
