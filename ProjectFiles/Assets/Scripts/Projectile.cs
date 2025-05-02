using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSettings settings = new ProjectileSettings();
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime * settings.projectileSpeed);
    }
}

[System.Serializable]
public class ProjectileSettings
{
    public float projectileSpeed = 8;
}