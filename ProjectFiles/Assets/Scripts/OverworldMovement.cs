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
    private Unit unit;
    public bool moving;

    private void Start()
    {
        transform.position = new Vector2(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);
        targetPos = transform.position;
    }

    void Update()
    {
        if (!moving)
            return;
        else
            spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y - 1f) * -1;
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            moving = false;
            SetSprite();
            spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y) * -1;
        }
    }
    
    public bool MovePos(Vector2 target)
    {
        facingPos = target;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingPos, 1.25f, mask);
        if (hit)
        {
            facingPos = target;
            SetSprite();
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
        this.unit = unit;
        animator.runtimeAnimatorController = unit.overworldAnimator;
    }

    public void SetSpriteLayer(int index)
    {
        spriteRenderer.sortingOrder = index;
    }

    public Vector2 GetFacingDir()
    {
        return facingPos;
    }
}
