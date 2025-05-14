using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShooter : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] public PatternSettings pattern;
    [HideInInspector] public int echoIndex;
    public virtual void OnEnable()
    {
        echoIndex = pattern.echoCount;
        ActivatePatternSpawner();
    }
    public void ActivatePatternSpawner()
    {
        ShootPattern(pattern);
    }

    public virtual void PatternEnded()
    {
        if (echoIndex > 0)
        {
            //yield return new WaitForSeconds(pattern.echoDelay);
            echoIndex--;
            ActivatePatternSpawner();
        }
        else
        {
            this.enabled = false;
        }
    }

    /// <summary>
    /// BulletPatternGen
    /// </summary>

    private int shotIndex;
    private void ShootPattern(PatternSettings setPattern)
    {
        pattern = setPattern;
        float shotPoint = transform.rotation.eulerAngles.z;
        if (pattern.burstCount > 1)
        {
            float angleStep = (float)pattern.burstRange / (pattern.burstCount - 1);
            float spacingApart = 0;
            float leftOffset = pattern.burstRange * -0.5f;
            Vector2 spawnPos = Vector2.zero;

            //Loop calcualating spawnpoint for each bullet
            for (int i = 0; i < pattern.burstCount; i++)
            {
                //yield return new WaitForSeconds(pattern.burstDelay);
                float angleDir = shotPoint + pattern.rotationOffset;
                if (pattern.randomBurst)
                    angleDir += Random.Range(-pattern.burstRange, pattern.burstRange);

                spawnPos = CalculateBurstPatternSpawnLoc(pattern, angleDir, spacingApart, leftOffset);
                if (pattern.burstPattern == Pattern.Radial && !pattern.radialShootStraight)
                    angleDir += spacingApart + leftOffset;

                CreateShot(spawnPos, angleDir + pattern.bulletRot, i);

                //Symmetry Calculations
                if (pattern.symmetry && pattern.rotationOffset != 0)
                {
                    angleDir = shotPoint - pattern.rotationOffset;

                    if (CalculateBurstPatternSpawnLoc(pattern, angleDir, -spacingApart, -leftOffset) != spawnPos)
                    {
                        spawnPos = CalculateBurstPatternSpawnLoc(pattern, angleDir, -spacingApart, -leftOffset);
                        if (pattern.burstPattern == Pattern.Radial && !pattern.radialShootStraight)
                            angleDir += -spacingApart + -leftOffset;

                        CreateShot(spawnPos, angleDir + pattern.bulletRot, i);
                    }
                }//End of symmetry Calc
                if (!pattern.randomBurst)
                    spacingApart += angleStep;
            }//End of burst loop
        }
        else
        {
            Vector2 spawnPos = new Vector2(transform.position.x + Mathf.Cos((shotPoint + pattern.rotationOffset) * Mathf.Deg2Rad) * pattern.spawnDistance, transform.position.y + Mathf.Sin((shotPoint + pattern.rotationOffset) * Mathf.Deg2Rad) * pattern.spawnDistance);
            CreateShot(spawnPos, shotPoint + pattern.rotationOffset + pattern.bulletRot, 0);
        }

        if (pattern.increaseShotIndexPerEcho)
        {
            shotIndex++;
            shotIndex %= pattern.shotArray.Length;
        }
        else
        {
            shotIndex = 0;
        }

        EndShot();
    }

    private GameObject groupingProjectile;
    private void CreateShot(Vector2 spawnPos, float rotation, int index)
    {
        if (pattern.shotArray.Length != 0)
        {
            if (pattern.createGroup && groupingProjectile == null)
            {
                groupingProjectile = new GameObject();
                groupingProjectile.transform.position = transform.position;
                groupingProjectile.transform.rotation = Quaternion.Euler(0, 0, 0);
                groupingProjectile.AddComponent<Projectile>().settings = pattern.groupedProjectileSettings;
            }

            if (pattern.randomizeShotArray)
                shotIndex = Random.Range(0, pattern.shotArray.Length);

            if (pattern.shotArray[shotIndex].createBullet)
            {
                GameObject bullet = ObjectPool.instance.GetPooledObject();
                bullet.SetActive(true);
                bullet.transform.position = spawnPos;
                bullet.transform.rotation = Quaternion.Euler(0, 0, rotation);
                bullet.transform.localScale = Vector3.one * pattern.shotArray[shotIndex].bulletSize;
                bullet.GetComponent<SpriteRenderer>().color = pattern.shotArray[shotIndex].color;
                bullet.GetComponent<SpriteRenderer>().sortingOrder = index;
                bullet.GetComponent<Bullet>().damageStats = new DamageStats(pattern.shotArray[shotIndex].damageStats);
                bullet.GetComponent<Bullet>().battleManager = battleManager;

                if (groupingProjectile != null)
                {
                    bullet.transform.parent = groupingProjectile.transform;
                }
                else if (pattern.shotArray[shotIndex].projectileSettings != null)
                {
                    bullet.GetComponent<Projectile>().settings = pattern.shotArray[shotIndex].projectileSettings;
                }

                bullet.layer = 9; //BossBulletlayer, Player Collision
            }

            if (pattern.increaseShotIndexPerBullet)
            {
                shotIndex++;
                shotIndex %= pattern.shotArray.Length;
            }
        }
        else
        {
            Debug.LogError("ShotArray seems empty");
        }
    }

    private void EndShot()
    {
        if (groupingProjectile != null)
        {
            groupingProjectile.AddComponent<GroupObject>();
            groupingProjectile = null;
        }
        PatternEnded();
    }

    private void OnDestroy()
    {
        if (groupingProjectile != null)
        {
            groupingProjectile.AddComponent<GroupObject>();
            groupingProjectile = null;
        }
    }

    private void OnDrawGizmosSelected() //Draws circles where the bullets will spawn in the editor, uses the same burst math as shoot bullet and also draws a line outwards based on its direction and speed
    {
        float shotPoint = transform.localRotation.eulerAngles.z;
        float firstBulletSpeed = 0;
        float firstBulletSize = 0.5f;
        if (pattern.shotArray.Length != 0)
        {
            firstBulletSpeed = pattern.shotArray[0].projectileSettings.projectileSpeed / 4;
            firstBulletSize = pattern.shotArray[0].bulletSize / 2;
        }

        if (pattern.burstCount > 1)
        {
            float angleStep = (float)pattern.burstRange / (pattern.burstCount - 1);
            float spacingApart = 0;
            float leftOffset = pattern.burstRange * -0.5f;
            Vector2 spawnPos = Vector2.zero;

            //Loop calcualating spawnpoint for each bullet
            for (int i = 0; i < pattern.burstCount; i++)
            {
                Gizmos.color = Color.red;
                float angleDir = shotPoint + pattern.rotationOffset;
                spawnPos = CalculateBurstPatternSpawnLoc(pattern, angleDir, spacingApart, leftOffset);
                Gizmos.DrawSphere(spawnPos, (firstBulletSize/3) + 0.0025f * i);
                if (pattern.burstPattern == Pattern.Radial && !pattern.radialShootStraight)
                    angleDir += spacingApart + leftOffset;
                Gizmos.DrawLine(spawnPos, new Vector2(spawnPos.x + Mathf.Sin((angleDir + pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed, spawnPos.y + Mathf.Cos((angleDir + pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed));

                //Symmetry Calculations
                if (pattern.symmetry && pattern.rotationOffset != 0)
                {
                    Gizmos.color = Color.blue;
                    angleDir = shotPoint - pattern.rotationOffset;

                    if (CalculateBurstPatternSpawnLoc(pattern, angleDir, -spacingApart, -leftOffset) != spawnPos)
                    {
                        spawnPos = CalculateBurstPatternSpawnLoc(pattern, angleDir, -spacingApart, -leftOffset);
                        Gizmos.DrawSphere(spawnPos, (firstBulletSize/3) + 0.0025f * i);

                        if (pattern.burstPattern == Pattern.Radial && !pattern.radialShootStraight)
                            angleDir += -spacingApart + -leftOffset;
                        Gizmos.DrawLine(spawnPos, new Vector2(spawnPos.x + Mathf.Sin((angleDir + -pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed, spawnPos.y + Mathf.Cos((angleDir + -pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed));
                    }
                }//End of symmetry Calc
                spacingApart += angleStep;
            }//End of burst loop
        }
        else
        {
            Vector2 spawnPos = new Vector2(transform.position.x + Mathf.Cos((shotPoint + pattern.rotationOffset) * Mathf.Deg2Rad) * pattern.spawnDistance, transform.position.y + Mathf.Sin((shotPoint + pattern.rotationOffset) * Mathf.Deg2Rad) * pattern.spawnDistance);
            Gizmos.DrawSphere(spawnPos, firstBulletSize);
            Gizmos.DrawLine(spawnPos, new Vector2(spawnPos.x + Mathf.Cos((shotPoint + pattern.rotationOffset + pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed, spawnPos.y + Mathf.Sin((shotPoint + pattern.rotationOffset + pattern.bulletRot) * Mathf.Deg2Rad) * firstBulletSpeed));
        }
    }

    private Vector2 CalculateBurstPatternSpawnLoc(PatternSettings patternSettingsForLoc, float angleDir, float spacingApart, float leftOffset)
    {
        Vector2 spawnPos = Vector2.zero;
        switch (patternSettingsForLoc.burstPattern)
        {
            case Pattern.Radial:
                spawnPos = new Vector2(transform.position.x + Mathf.Sin((angleDir + spacingApart + leftOffset) * Mathf.Deg2Rad) * patternSettingsForLoc.spawnDistance, transform.position.y + Mathf.Cos((angleDir + spacingApart + leftOffset) * Mathf.Deg2Rad) * patternSettingsForLoc.spawnDistance);
                break;
            case Pattern.Line:
                Vector2 midStartPoint = new Vector2(transform.position.x + Mathf.Sin(angleDir * Mathf.Deg2Rad) * patternSettingsForLoc.spawnDistance, transform.position.y + Mathf.Cos(angleDir * Mathf.Deg2Rad) * patternSettingsForLoc.spawnDistance);
                Vector2 lineStartPoint = new Vector2(midStartPoint.x + Mathf.Cos((angleDir) * Mathf.Deg2Rad) * leftOffset, midStartPoint.y + Mathf.Sin((angleDir) * Mathf.Deg2Rad) * leftOffset);
                spawnPos = new Vector2(lineStartPoint.x + Mathf.Cos((angleDir) * Mathf.Deg2Rad) * spacingApart, lineStartPoint.y + Mathf.Sin((angleDir) * Mathf.Deg2Rad) * spacingApart);
                break;
            default:
                Debug.LogError("Burst Pattern gizmos aint goin through :/");
                break;
        }
        return spawnPos;
    }
}

public enum Pattern
{
    Radial,
    Line
}

[System.Serializable]
public class PatternSettings
{
    [Header("Shooter Settings")]
    [SerializeField, Range(-360f, 360f)] public float rotationOffset;
    [SerializeField, Range(-180f, 180f)] public float bulletRot;
    [SerializeField] public float spawnDistance;

    [Header("Shot Settings")]
    public ShotSettings[] shotArray = { new ShotSettings() };
    public bool randomizeShotArray;
    public bool increaseShotIndexPerBullet;
    public bool increaseShotIndexPerEcho;

    [Header("Burst Settings")]
    public Pattern burstPattern;
    public int burstCount = 1;
    public float burstRange;
    public float burstDelay;
    public bool randomBurst;
    public bool radialShootStraight;
    public bool symmetry;

    [Header("Echo Settings")]
    public int echoCount;
    public float echoDelay;

    [Header("Group Settings")]
    public bool createGroup;
    public ProjectileSettings groupedProjectileSettings;
}

[System.Serializable]
public class ShotSettings
{
    public bool createBullet = true;
    public ProjectileSettings projectileSettings;
    public Color color;
    public DamageStats damageStats;
    [Range(0f, 2f)] public float bulletSize = 0.5f;
}
