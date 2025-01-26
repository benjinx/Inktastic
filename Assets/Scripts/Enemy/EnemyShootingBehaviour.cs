using Unity.VisualScripting;
using UnityEngine;

public class EnemyShootingBehaviour : GameplayBehaviour
{
    public int burstSize;
    public float burstInterval;
    public Bubble.Mode trackingMode;
    public Transform firePoint;
    public float bubbleSpeed;
    public float bubbleLife = 5f;

    [HideInInspector]
    public EnemyStateMachine esm;

    private float currentBurstTime;
    private bool attacking;
    private int currentBurst = 0;

    public void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
    }

    public void Update()
    {
        if (attacking)
        {
            currentBurstTime += Time.deltaTime;

            if(currentBurstTime >= burstInterval)
            {
                currentBurst++;
                currentBurstTime = 0;
                LaunchBubble();

                if(currentBurst >= burstSize)
                {
                    attacking = false;
                }
            }
        }
    }

    public void LaunchBubble()
    {
        Bubble bubble = ObjectPooler.Instance.SpawnFromPool("bubbles", firePoint.transform.position, Quaternion.Euler(firePoint.forward)).GetComponent<Bubble>();
        bubble.speed = bubbleSpeed;
        bubble.mode = trackingMode;
        bubble.transform.forward = firePoint.transform.forward;
        if(esm.eyes.activeTarget != null)
        {
            bubble.target = esm.eyes.activeTarget.transform;
        }

        bubble.combatHandler = GetComponent<CombatHandler>();
        bubble.dumbInitializeHack = true;

        Timer time = bubble.AddComponent<Timer>();
        time.duration = bubbleLife;
        time.startAction = Timer.StartAction.OnlyInvokeEvent;
        time.onFinishedInternal += bubble.Despawn;
        time.EnableTimer();
    }

    public void InitiateAttack()
    {
        if(esm.eyes.activeTarget != null)
        {
            currentBurst = 0;
            currentBurstTime = 0;
            attacking = true;
        }
    }
}
