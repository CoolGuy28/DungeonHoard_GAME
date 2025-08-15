using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMovement : MonoBehaviour
{
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private LayerMask mask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Unit unit;

    private void Start()
    {
        targetPos = transform.position;
    }
    public bool MovePos(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target, 1.25f, mask);
        if (hit)
        {
            return true;
        }
        else
        {
            targetPos += target;
            transform.position = targetPos;
            if (target.x < 0)
                spriteRenderer.sprite = unit.overWorldSprites[3];
            else if (target.x > 0)
                spriteRenderer.sprite = unit.overWorldSprites[2];
            else if (target.y > 0)
                spriteRenderer.sprite = unit.overWorldSprites[1];
            else
                spriteRenderer.sprite = unit.overWorldSprites[0];

            return false;
        }
    }

    public void Interact(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target, 1.25f, mask);
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
