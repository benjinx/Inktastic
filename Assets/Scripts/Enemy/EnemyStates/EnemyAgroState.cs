using UnityEngine;

[System.Serializable]
public class EnemyAgroState : EnemyState
{
    public float agroSpeed;
    public float attackDistance;

    [Range(0,100)]
    public float attackProbability;
    public float attackCooldown;
    public float investigateTime;
    public float stopDistance;

    private Vector3 targetPosition;
    private float currentInvestigateTime;
    private float currentAttackCooldown;
    public bool attackOnCooldown;

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (attackOnCooldown)
        {
            currentAttackCooldown += Time.deltaTime;
            if(currentAttackCooldown >= attackCooldown)
            {
                attackOnCooldown = false;
            }
        }

        if(esm.eyes.activeTarget != null)
        {
            if (esm.eyes.canSeeCurrentTarget)
            {
                //chase the enemy
                targetPosition = esm.eyes.activeTarget.transform.position;
            }
            else
            {
                //go toward the enemy's last known position
                targetPosition = esm.eyes.lastSeenPos;
            }

            Vector3 targetDir = (targetPosition - esm.transform.position).normalized;

            esm.currentLookAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;

            if(esm.eyes.distanceToTarget <= attackDistance && !attackOnCooldown)
            {
                AttackRoll();
            }

            if(Vector3.Distance(esm.transform.position, targetPosition) >= stopDistance)
            {
                esm.cCon.Move(targetDir * agroSpeed * Time.deltaTime);

            }
            //if we've reached our last known target position, wait a bit before we decide to go back into patroling
            else
            {
                if (!esm.eyes.canSeeCurrentTarget)
                {
                    currentInvestigateTime += Time.deltaTime;

                    if(currentInvestigateTime >= investigateTime)
                    {
                        esm.ChangeState(esm.patrolState);
                    }
                }
                else
                {
                    currentInvestigateTime = 0;
                }
            }
        }
        else
        {
            esm.ChangeState(esm.patrolState);
        }
    }

    public void AttackRoll()
    {
        if(Random.Range(0,100) <= attackProbability)
        {
            attackOnCooldown = true;
            currentAttackCooldown = 0;
            esm.TryChangeState(esm.attackState);
        }
    }

    public override void OnStateExit()
    {
        base.OnStateExit();

        currentInvestigateTime = 0;
    }
}
