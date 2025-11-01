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
    private Sprite[] unitSprites;
    private bool flipLeft;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float sprintMultiplier = 1.4f;
    [SerializeField] private float sortingPriority;
    private float currentSpeed;
    public bool moving;
    private AudioSource audioSource;
    private void Start()
    {
        transform.position = new Vector2(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);
        targetPos = transform.position;
        currentSpeed = speed;
        if (GetComponent<AudioSource>())
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        SetSpriteLayer();
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
            if (audioSource != null)
                audioSource.Pause();
            SetSprite();
            if (animator != null)
                animator.speed = 0;
            SetSpriteLayer();
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
            if (animator != null)
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
                if (animator != null)
                    animator.speed = 1;
                moving = true;
                if (audioSource != null)
                    audioSource.Play();
                return false;
            }
        }
    }

    private void SetSprite()
    {
        if (animator != null)
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
        }
        else
        {
            if (facingPos.y < 0)
            {
                spriteRenderer.sprite = unitSprites[0];
                spriteRenderer.flipX = false;
            }
            else if (facingPos.x < 0)
            {
                if (flipLeft)
                {
                    spriteRenderer.sprite = unitSprites[2];
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.sprite = unitSprites[3];
                    spriteRenderer.flipX = false;
                }
            }
            else if (facingPos.x > 0)
            {
                spriteRenderer.sprite = unitSprites[2];
                spriteRenderer.flipX = false;
            }
            else if (facingPos.y > 0)
            {
                spriteRenderer.sprite = unitSprites[1];
                spriteRenderer.flipX = false;
            }
        }


        SetSpriteLayer();
    }

    public RaycastHit2D Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingPos, 1.25f, mask);
        return hit;
    }

    public void SetUnit(Unit unit)
    {
        if (unit.overworldAnimator != null)
            animator.runtimeAnimatorController = unit.overworldAnimator;
        else
        {
            animator = null;
            flipLeft = unit.flipLeft;
            unitSprites = unit.nonAnimatedSprites;
        }
            
    }

    public void SetSpriteLayer()
    {
        spriteRenderer.sortingOrder = Mathf.FloorToInt(transform.position.y - 0.75f + -sortingPriority) * -1;
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
