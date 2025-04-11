using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    private void OnCollisionExit2D(Collision2D collision)
    {
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        StopAllCoroutines();
        gameObject.transform.parent = ObjectPool.instance.transform;
        gameObject.SetActive(false);
    }
}
