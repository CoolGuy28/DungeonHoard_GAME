using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyObject : MonoBehaviour
{
    [SerializeField] private EnemyOverworldData data;
    [SerializeField] private SpriteRenderer spriteObj;
    public int index;

    private void Start()
    {
        if (data.enemyFight != null)
            spriteObj.sprite = data.enemyFight[data.displaySprite].unit.battleSprite;
        spriteObj.sortingOrder = index;
        if (data.dead)
        {
            spriteObj.color = Color.red;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void SetEnemyData(EnemyOverworldData data, int index)
    {
        this.data.SetOverworldData(data);
        this.index = index;
        if (this.data.dead)
        {
            spriteObj.color = Color.red;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public List<CharacterData> GetEnemies()
    {
        return data.enemyFight;
    }
}
