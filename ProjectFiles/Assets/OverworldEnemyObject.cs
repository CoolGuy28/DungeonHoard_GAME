using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyObject : MonoBehaviour
{
    [SerializeField] private List<CharacterData> enemyFight;
    [SerializeField] private SpriteRenderer spriteObj;
    public Vector2 enemyPosition;
    public bool dead;

    private void Start()
    {
        if (enemyFight != null)
            spriteObj.sprite = enemyFight[0].unit.battleSprite;
        if (dead)
            spriteObj.color = Color.red;
        transform.position = enemyPosition;
    }

    public List<CharacterData> GetEnemies()
    {
        return enemyFight;
    }
}
