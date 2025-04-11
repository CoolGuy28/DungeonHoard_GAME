using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSettings settings = new ProjectileSettings();
    public bool active;
    //private int currentCommand;

    private void OnEnable()
    {
        active = false;
    }
    private void FixedUpdate()
    {
        if (!active)
            return;
        //Movement
        if (settings.ignoreRotation)
        {
            transform.position += Vector3.down * Time.deltaTime * settings.projectileSpeed;
        }
        else
        {
            transform.Translate(Vector2.down * Time.deltaTime * settings.projectileSpeed);
        }
        
        transform.Translate(Vector2.up * Time.deltaTime * settings.sidewaysProjectileSpeed);

        if (settings.changeSpeedOverTime && !settings.reachedTargetSpeed)
        {
            if (settings.projectileSpeed < settings.targetSpeed)
            {
                settings.projectileSpeed += settings.changeSpeedRate;
                if (settings.projectileSpeed > settings.targetSpeed)
                    settings.reachedTargetSpeed = true;
            }
            else if (settings.projectileSpeed > settings.targetSpeed)
            {
                settings.projectileSpeed += settings.changeSpeedRate;
                if (settings.projectileSpeed < settings.targetSpeed)
                    settings.reachedTargetSpeed = true;
            }
            if (settings.flipBetweenSpeeds)
            {
                settings.targetSpeed *= -1;
                settings.reachedTargetSpeed = false;
            }
        }

        //Homing
        if (settings.homing)
        {
            if (settings.targetPos != null)
            {
                Vector2 direction = (Vector2)settings.targetPos.position - (Vector2)transform.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z - rotateAmount * settings.rotSpeed);
            }
            else
                FindTarget();
        }

        //Rotation Over Time
        if (settings.rotSpeed != 0 && !settings.homing)
            transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + settings.rotSpeed);
    }

    public void StartDelayActivation()
    {
        StartCoroutine(ActivateOnDelay());
    }

    private IEnumerator ActivateOnDelay()
    {
        yield return new WaitForSeconds(settings.activeDelay);
        active = true;
    }

    /*private IEnumerator PlayCommand()
    {
        //If a list of commands is set in the bullet builder it plays through them one by one with a set time
        yield return new WaitForSeconds(settings.commandsList[currentCommand].timeBeforeCommand);
        switch (settings.commandsList[currentCommand].command)
        {
            case BulletMethods.SetSpeed:
                SetSpeed(settings.commandsList[currentCommand].value);
                break;
            case BulletMethods.SetTargetSpeed:
                settings.targetSpeed = settings.commandsList[currentCommand].value;
                settings.changeSpeedOverTime = true;
                break;
            case BulletMethods.SetSidewaysSpeed:
                settings.sidewaysProjectileSpeed = settings.commandsList[currentCommand].value;
                break;
            case BulletMethods.SetRotationSpeed:
                settings.rotSpeed = settings.commandsList[currentCommand].value;
                break;
            case BulletMethods.SetToStartRot:
                SetRotation(settings.startRot);
                break;
            case BulletMethods.SetToShooterRot:
                SetRotation(settings.shooterRot);
                break;
            case BulletMethods.StartHomingPlayer:
                settings.homing = true;
                settings.rotSpeed = settings.commandsList[currentCommand].value;
                FindTarget();
                break;
            case BulletMethods.StopHoming:
                settings.homing = false;
                settings.rotSpeed = 0;
                break;
            case BulletMethods.SetRepetition:
                RepeatFutureCommands();
                break;
            case BulletMethods.DestroyBullet:
                DestroyBullet();
                break;
            default:
                Debug.LogError("Command Not Set or smthn ¯_(o. o)_/¯");
                break;
        }
        currentCommand++;
        if (currentCommand == settings.commandsList.Length)
        {
            if (settings.repeatCommand)//command list can be set to repeat otherwise it'll end here
            {
                currentCommand = settings.repetitionIndex;
                StartCoroutine(PlayCommand());
            }
        }
        else
        {
            StartCoroutine(PlayCommand());
        }
    }

    public void SetCommands(BulletCommands[] newCommands)
    {
        if (newCommands.Length != 0)
        {
            currentCommand = 0;
            settings.commandsList = newCommands;
            StartCoroutine(PlayCommand());
        }
        else
        {
            Debug.LogError("Oopsie doopsie smthns wrong commands list aint set");
        }
    }

    private void SetSpeed(float speed)
    {
        settings.projectileSpeed = speed;
    }

    private void SetRotation(float rot)
    {
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }
    /*/
    private void FindTarget()
    {
        if (gameObject.layer == 9)
        {
            if (GameObject.FindWithTag("Player") != null)
            {
                settings.targetPos = GameObject.FindWithTag("Player").transform;
            }
        }
        else if (gameObject.layer == 10)
        {
            if (GameObject.FindWithTag("Enemy") != null)
            {
                settings.targetPos = GameObject.FindWithTag("Enemy").transform;
            }
        }
    }
    /*
    private void RepeatFutureCommands()
    {
        settings.repetitionIndex = currentCommand + 1;
        settings.repeatCommand = true;
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
    }*/
}

/*public enum BulletMethods
{
    SetSpeed,
    SetTargetSpeed,
    SetSidewaysSpeed,
    SetRotationSpeed,
    SetToStartRot,
    SetToShooterRot,
    StartHomingPlayer,
    StopHoming,
    SetRepetition,
    DestroyBullet
}

[System.Serializable]
public struct BulletCommands
{
    public BulletMethods command;
    public float value;
    public float timeBeforeCommand;
}*/

[System.Serializable]
public class ProjectileSettings
{
    public float activeDelay;
    public bool ignoreRotation;

    public float projectileSpeed = 8;
    public bool changeSpeedOverTime;
    public bool reachedTargetSpeed;
    public bool flipBetweenSpeeds;
    public float targetSpeed;
    public float changeSpeedRate = 0.1f;
    public float sidewaysProjectileSpeed;

    public bool homing;
    public Transform targetPos;

    public float rotSpeed;
    public float startRot;
    public float shooterRot;

    //public bool playCommands;
    //public BulletCommands[] commandsList;

    //public bool repeatCommand;
    //public int repetitionIndex;
}