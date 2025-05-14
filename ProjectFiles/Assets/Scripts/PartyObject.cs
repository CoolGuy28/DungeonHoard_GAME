using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyObject : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private SpriteRenderer spriteObj;
    private Rigidbody2D rb;
    //[SerializeField] private GameObject followerPrefab;
    //[SerializeField] private List<GameObject> partyObjects;
    //[SerializeField] private Vector2[] movementHis = new Vector2[100];
    //private int followDis = 25;
    private bool allowMovement = true;
    private GameObject interactableObject;

    private void Start()
    {
        //transform.position = GameManager.instance.partyPosition;
        rb = GetComponent<Rigidbody2D>();
        //CreateOverworldParty();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.LoadMenu();
        }

        if(allowMovement)
        {
            Vector2 movement = new Vector2(0f, 0f);
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement = new Vector2(-1, 0);
                spriteObj.GetComponent<SpriteRenderer>().sprite = GameManager.instance.party[0].unit.overWorldSprites[3];
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement = new Vector2(1, 0);
                spriteObj.GetComponent<SpriteRenderer>().sprite = GameManager.instance.party[0].unit.overWorldSprites[2];
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement = new Vector2(0, 1);
                spriteObj.GetComponent<SpriteRenderer>().sprite = GameManager.instance.party[0].unit.overWorldSprites[1];
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                movement = new Vector2(0, -1);
                spriteObj.GetComponent<SpriteRenderer>().sprite = GameManager.instance.party[0].unit.overWorldSprites[0];
            }

            rb.MovePosition((Vector2)transform.position + movement * Time.fixedDeltaTime * moveSpeed);
            //transform.Translate(movement * Time.deltaTime * moveSpeed);
            /*if (movement != Vector2.zero)
            {
                AddMovementHistory(movement);
                for(int i = 0; i < partyObjects.Count; i++)
                {
                    partyObjects[i].transform.Translate(movementHis[(i+1)*followDis] * Time.deltaTime * moveSpeed);
                }
            }*/
            
        }
        
    }

    /*private void AddMovementHistory(Vector2 startHis)
    {
        Vector2 savedHis = startHis;
        Vector2 savedHis2;
        for(int i = 0; i < movementHis.Length; i++)
        {
            savedHis2 = movementHis[i];
            movementHis[i] = savedHis;
            savedHis = savedHis2;
        }
    }

    private void CreateOverworldParty()
    {
        for(int i = 0; i < GameManager.instance.party.Count; i++)
        {
            if (i == 0)
            {
                spriteObj.sprite = GameManager.instance.party[0].unit.battleSprite;
            }
            else
            {
                GameObject a = Instantiate(followerPrefab, transform.position, Quaternion.identity);
                a.GetComponent<SpriteRenderer>().sprite = GameManager.instance.party[i].unit.battleSprite;
                partyObjects.Add(a);
            }
        }
    }*/


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            allowMovement = false;
            GameManager.instance.partyPosition = transform.position;
            GameManager.instance.LoadBattle(collision.gameObject.GetComponent<OverworldEnemyObject>().index);
        }
    }
}
