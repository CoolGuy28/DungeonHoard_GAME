using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyFollower : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<OverworldEnemyObject>())
        {
            GameManager.instance.partyPosition = transform.position;
            GameManager.instance.LoadBattle(collision.gameObject.GetComponent<OverworldEnemyObject>().GetStartingLoc());
            GameManager.instance.SaveGame();
        }
    }
}
