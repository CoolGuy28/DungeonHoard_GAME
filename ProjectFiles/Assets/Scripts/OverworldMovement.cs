using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMovement : MonoBehaviour
{
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private Vector2 facingPos;
    [SerializeField] private LayerMask mask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float sprintMultiplier = 1.4f;
    private float currentSpeed;
    public bool moving;

    private void Start()
    {
        transform.position = new Vector2(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);
        targetPos = transform.position;
        currentSpeed = speed;
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y - 1f) * -1;
    }

    void FixedUpdate()
    {
        if (!moving)
            return;

        var step = currentSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            moving = false;
            //animator.Rebind();
            SetSprite();
            animator.speed = 0;
            spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y) * -1;
        }
    }
    
    public bool MovePos(Vector2 target, bool sprinting)
    {
        SetSprint(sprinting);
        facingPos = target;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingPos, 1.25f, mask);
        if (hit)
        {
            facingPos = target;
            //animator.Rebind();
            SetSprite();
            animator.speed = 0;
            return true;
        }
        else
        {
            if (moving)
            {
                return true;
            }
            else
            {
                targetPos += target;
                facingPos = target;
                SetSprite();
                animator.speed = 1;
                moving = true;

                return false;
            }
        }
    }

    private void SetSprite()
    {
        if (facingPos.y < 0)
            animator.SetInteger("Movement", 1);
        else if (facingPos.x < 0)
            animator.SetInteger("Movement", 2);
        else if (facingPos.x > 0)
            animator.SetInteger("Movement", 3);
        else if (facingPos.y > 0)
            animator.SetInteger("Movement", 4);
        else
            animator.SetInteger("Movement", 0);

        spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y - 1f) * -1;
    }

    public RaycastHit2D Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingPos, 1.25f, mask);
        return hit;
    }

    public void SetUnit(Unit unit)
    {
        animator.runtimeAnimatorController = unit.overworldAnimator;
    }

    public void SetSpriteLayer(int index)
    {
        spriteRenderer.sortingOrder = index;
    }

    public void SetSprint(bool sprinting)
    {
        currentSpeed = speed;
        if (sprinting)
            currentSpeed *= sprintMultiplier;
    }

    public Vector2 GetFacingDir()
    {
        return facingPos;
    }

    public bool GetMovingStatus()
    {
        return moving;
    }
}
