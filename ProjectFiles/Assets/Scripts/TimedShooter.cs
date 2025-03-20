using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedShooter : BaseShooter
{
    [SerializeField] private float startShotTime;
    [SerializeField] private float timeBtwShots;
    [SerializeField] private float randomOffset;
    [SerializeField] private bool increasePatPerShot;

    public override void OnEnable()
    {
        echoIndex = patterns[powerLevel].echoCount;
        StartCoroutine(BeginShot(Random.Range(startShotTime - randomOffset, startShotTime + randomOffset))); //starts shooting the bulletbuilder once start time runs out
    }

    private IEnumerator BeginShot(float timer)
    {
        yield return new WaitForSeconds(timer);
        ActivatePatternSpawner();
    }

    public override IEnumerator PatternEnded()
    {
        if (echoIndex > 0)
        {
            yield return new WaitForSeconds(patterns[powerLevel].echoDelay);
            echoIndex--;
            ActivatePatternSpawner();
        }
        else
        {
            echoIndex = patterns[powerLevel].echoCount;
            powerLevel++;
            if (powerLevel >= patterns.Length)
                powerLevel = 0;
            StartCoroutine(BeginShot(Random.Range(timeBtwShots - randomOffset, timeBtwShots + randomOffset))); //when the bulletbuilder ends it shoots again with timeBtwShot
        }
    }
}
