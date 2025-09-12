using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyObject : MonoBehaviour, IDataPersistence
{
    [SerializeField] private EnemyOverworldData enemyData;
    [SerializeField] private SpriteRenderer spriteObj;
    [SerializeField] private EnemyMovementAI movementAI;
    public int index = -1;
    [SerializeField] private OverworldEnemyObject[] linkedEnemies;

    private void Start()
    {
        spriteObj.sortingOrder = index;
        if (enemyData.dead)
        {
            SetEnemyDead();
            foreach (OverworldEnemyObject en in linkedEnemies)
                en.SetEnemyDead();
        }
    }

    public void SetEnemyData(EnemyOverworldData enemyData)
    {
        this.enemyData.SetOverworldData(enemyData);
        transform.position = enemyData.enemyPosition;
        if (this.enemyData.dead)
        {
            SetEnemyDead();
            foreach (OverworldEnemyObject en in linkedEnemies)
                en.SetEnemyDead();
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

    public void SetEnemyDead()
    {
        enemyData.dead = true;
        if (movementAI != null)
            movementAI.enabled = false;
        spriteObj.color = Color.red;
        gameObject.tag = "Interactable";
    }
}
