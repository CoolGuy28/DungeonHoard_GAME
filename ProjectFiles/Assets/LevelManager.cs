using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    [SerializeField] private List<OverworldEnemyObject> enemyObjects = new List<OverworldEnemyObject>();

    private void Awake()
    {
        _instance = this;
        foreach(OverworldEnemyObject e in GameObject.FindObjectsOfType<OverworldEnemyObject>())
        {
            enemyObjects.Add(e);
        }
    }
}
