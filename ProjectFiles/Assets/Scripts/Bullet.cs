using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public DamageStats damageStats;
    public BattleManager battleManager;
    private void OnCollisionExit2D(Collision2D collision)
    {
        DestroyBullet();
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
        gameObject.transform.parent = ObjectPool.instance.transform;
        gameObject.SetActive(false);
    }
}
