using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMovement : MonoBehaviour
{
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private Vector2 facingPos;
    [SerializeField] private LayerMask mask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 0.5f;
    private Unit unit;
    private bool moving;

    private void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (!moving)
            return;
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            moving = false;
        }
    }
    
    public bool MovePos(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target, 1.25f, mask);
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
        if (facingPos.x < 0)
            spriteRenderer.sprite = unit.overWorldSprites[3];
        else if (facingPos.x > 0)
            spriteRenderer.sprite = unit.overWorldSprites[2];
        else if (facingPos.y > 0)
            spriteRenderer.sprite = unit.overWorldSprites[1];
        else
            spriteRenderer.sprite = unit.overWorldSprites[0];
    }

    public void Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingPos, 1.25f, mask);
        if (hit)
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                hit.collider.GetComponent<Interactable>().BeginDialogue();
            }
                
        }
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        spriteRenderer.sprite = spriteRenderer.sprite = unit.overWorldSprites[0];
    }

    public void SetSpriteLayer(int index)
    {
        spriteRenderer.sortingOrder = index;
    }
}
