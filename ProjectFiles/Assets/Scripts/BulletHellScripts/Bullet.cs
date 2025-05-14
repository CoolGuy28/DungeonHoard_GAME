using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public DamageStats damageStats;
    public BattleManager battleManager;
    private int wallCol = 0;
    private void OnCollisionExit2D(Collision2D collision)
    {
        wallCol++;
        if (wallCol >= 2)
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            battleManager.MagicAttackHit(new DamageStats(damageStats));
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        wallCol = 0;
        gameObject.transform.parent = ObjectPool.instance.transform;
        gameObject.SetActive(false);
    }
}
