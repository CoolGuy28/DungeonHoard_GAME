using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBirdManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private Projectile noose;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float timeBtwSpawn;
    private void OnEnable()
    {
        StartCoroutine(SpawnNoose());
    }

    private IEnumerator SpawnNoose()
    {
        yield return new WaitForSeconds(timeBtwSpawn);
        Projectile newNoose = Instantiate(noose, new Vector2(spawnArea.position.x, Random.Range(-spawnArea.position.y, spawnArea.position.y)), Quaternion.identity, transform);
        newNoose.settings.projectileSpeed = moveSpeed;
        StartCoroutine(SpawnNoose());
    }
}
