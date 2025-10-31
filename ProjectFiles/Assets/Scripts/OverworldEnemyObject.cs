using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyObject : MonoBehaviour, IDataPersistence
{
    [SerializeField] private EnemyOverworldData enemyData;
    [SerializeField] private SpriteRenderer spriteObj;
    [SerializeField] private EnemyMovementAI movementAI;
    [SerializeField] private OverworldEnemyObject[] linkedEnemies;

    private void Awake()
    {
        enemyData.startingPos = transform.position;
    }
    private void Start()
    {
        if (enemyData.dead)
        {
            SetEnemyDead();
            foreach (OverworldEnemyObject en in linkedEnemies)
                en.SetEnemyDead();
        }
    }

    public void SetEnemyData(EnemyOverworldData enemyData)
    {
        new EnemyOverworldData(enemyData);
        transform.position = enemyData.enemyPosition;
        this.enemyData.dead = enemyData.dead;
        if (this.enemyData.dead)
        {
            SetEnemyDead();
            foreach (OverworldEnemyObject en in linkedEnemies)
                en.SetEnemyDead();
        }
    }

    public void LoadData(GameData data)
    {
        EnemyOverworldData foundCopy = null;
        foreach (EnemyOverworldData e in data.sceneData[data.sceneIndex].enemies)
        {
            if (e.SameStartingPos(enemyData.startingPos) != null)
            {
                foundCopy = e;
                break;
            }
        }
        if (foundCopy == null)
        {
            enemyData.enemyPosition = transform.position;
            data.sceneData[data.sceneIndex].SaveNewEnemy(new EnemyOverworldData(enemyData));
        }
        else
        {
            SetEnemyData(foundCopy);
        }
    }

    public void SaveData(GameData data)
    {
        EnemyOverworldData foundCopy = null;
        foreach (EnemyOverworldData e in data.sceneData[data.sceneIndex].enemies)
        {
            if (e.SameStartingPos(enemyData.startingPos) != null)
            {
                foundCopy = e;
                break;
            }
        }
        enemyData.enemyPosition = transform.position;
        if (foundCopy == null)
            data.sceneData[data.sceneIndex].enemies.Add(new EnemyOverworldData(enemyData));
        else
            foundCopy = new EnemyOverworldData(enemyData);
    }

    public Vector2 GetStartingLoc()
    {
        return enemyData.startingPos;
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
        spriteObj.sprite = enemyData.enemyFight[0].unit.overworldDeathSprite;
        spriteObj.sortingLayerID = 0;
        spriteObj.sortingOrder = -1;
        gameObject.tag = "Interactable";
    }
}
