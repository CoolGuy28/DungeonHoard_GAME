using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyObject : MonoBehaviour, IDataPersistence
{
    [SerializeField] private EnemyOverworldData enemyData;
    [SerializeField] private SpriteRenderer spriteObj;
    public int index = -1;

    private void Start()
    {
        if (enemyData.enemyFight != null)
            spriteObj.sprite = enemyData.enemyFight[enemyData.displaySprite].unit.battleSprite;
        spriteObj.sortingOrder = index;
        if (enemyData.dead)
        {
            spriteObj.color = Color.red;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void SetEnemyData(EnemyOverworldData enemyData)
    {
        this.enemyData.SetOverworldData(enemyData);
        transform.position = enemyData.enemyPosition;
        if (this.enemyData.dead)
        {
            spriteObj.color = Color.red;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void LoadData(GameData data, int index)
    {
        this.index = index;
        if (index >= data.sceneData[data.sceneIndex].enemies.Count)
        {
            enemyData.enemyPosition = transform.position;
            EnemyOverworldData newData = new EnemyOverworldData();
            newData.SetOverworldData(enemyData);
            data.sceneData[data.sceneIndex].SaveNewEnemy(newData);
        }
        else
        {
            SetEnemyData(data.sceneData[data.sceneIndex].enemies[index]);
        }
    }

    public void SaveData(GameData data)
    {
        enemyData.enemyPosition = transform.position;
        data.sceneData[data.sceneIndex].enemies[index].SetOverworldData(enemyData);
    }

    public List<CharacterData> GetEnemies()
    {
        return enemyData.enemyFight;
    }
}
