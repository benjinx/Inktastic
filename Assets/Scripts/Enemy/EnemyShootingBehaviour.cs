using UnityEngine;

public class EnemyShootingBehaviour : GameplayBehaviour
{
    public int burstSize;
    public float burstInterval;
    public Bubble.Mode trackingMode;
    public Transform firePoint;

    [HideInInspector]
    public EnemyStateMachine esm;

    private ObjectPooler pooler;
    private float currentBurstTime;
    private bool attacking;
    private int currentBurst = 0;

    public void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
        pooler = GetComponent<ObjectPooler>();
    }

    public void Update()
    {
        if (attacking)
        {
            currentBurstTime += Time.deltaTime;

            if(currentBurstTime >= burstInterval)
            {
                currentBurst++;
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
        pooler.SpawnFromPool("bubbles", firePoint.transform.position, Quaternion.Euler(Vector3.forward));
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
