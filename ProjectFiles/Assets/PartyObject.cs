using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyObject : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        transform.position = GameManager.instance.partyPosition;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.LoadMenu();
        }

        Vector2 movement = new Vector2(0f, 0f);
        if (Input.GetKey(KeyCode.LeftArrow))
            movement = new Vector2(-1, 0);

        if (Input.GetKey(KeyCode.RightArrow))
            movement = new Vector2(1, 0);

        if (Input.GetKey(KeyCode.UpArrow))
            movement = new Vector2(0, 1);

        if (Input.GetKey(KeyCode.DownArrow))
            movement = new Vector2(0, -1);

        transform.Translate(movement * Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!collision.GetComponent<OverworldEnemyObject>().dead)
            {
                GameManager.instance.partyPosition = transform.position;
                GameManager.instance.LoadBattle(collision.GetComponent<OverworldEnemyObject>());
            }
        }
    }
}
