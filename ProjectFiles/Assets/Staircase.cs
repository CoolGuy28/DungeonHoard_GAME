using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    [SerializeField] private int sceneChange;
    [SerializeField] private Vector2 position;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.ChangeGameScene(sceneChange);
            GameManager.instance.gameData.playerPos = position;
        }
    }
}
