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
    public float hangBackMaxDistance;
    public float hangBackMinDistance;
    public float circleSwitchCooldown = 3f; // Time between direction switches
    private float currentCircleSwitchTime = 0f;
    private int circleDirection = 1; // 1 for clockwise, -1 for counterclockwise


    public MoveObjective moveObjective;

    public enum MoveObjective
    {
        Rush,
        HangBack,
        Stationary
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        if(esm.eyes.activeTarget != null)
        {
            Vector3 targetDir = (esm.eyes.activeTarget.transform.position - esm.transform.position).normalized;

            esm.currentLookAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        }

    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (attackOnCooldown)
        {
            currentAttackCooldown += Time.deltaTime;
            if (currentAttackCooldown >= attackCooldown)
            {
                attackOnCooldown = false;
            }
        }

        if (esm.eyes.activeTarget != null)
        {
            if (esm.eyes.canSeeCurrentTarget)
            {
                // Set target to current enemy position
                targetPosition = esm.eyes.activeTarget.transform.position;
            }
            else
            {
                // Go to last known position
                targetPosition = esm.eyes.lastSeenPos;
            }

            Vector3 targetDir = (targetPosition - esm.transform.position).normalized;

            esm.currentLookAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;

            switch (moveObjective)
            {
                case MoveObjective.Rush:
                    HandleRush(targetDir);
                    break;

                case MoveObjective.HangBack:
                    HandleHangBack(targetDir);
                    break;

                case MoveObjective.Stationary:
                    HandleStationary();
                    break;
            }
        }
        else
        {
            esm.ChangeState(esm.patrolState);
        }
    }

    // Rush behavior
    private void HandleRush(Vector3 targetDir)
    {
        if (esm.eyes.distanceToTarget <= attackDistance && !attackOnCooldown)
        {
            AttackCheck();
        }

        if (Vector3.Distance(esm.transform.position, targetPosition) >= stopDistance)
        {
            esm.cCon.Move(targetDir * agroSpeed * Time.deltaTime);
        }
        else if (!esm.eyes.canSeeCurrentTarget)
        {
            HandleInvestigate();
        }
    }

    // HangBack behavior
    private void HandleHangBack(Vector3 targetDir)
    {
        float distanceToTarget = Vector3.Distance(esm.transform.position, targetPosition);

        // Maintain a desired distance
        if (distanceToTarget < hangBackMinDistance)
        {
            // Move away from the player
            Vector3 moveDir = -(targetPosition - esm.transform.position).normalized;
            esm.cCon.Move(moveDir * agroSpeed * Time.deltaTime);
        }
        else if (distanceToTarget > hangBackMaxDistance)
        {
            // Move closer to maintain distance
            esm.cCon.Move(targetDir * agroSpeed * Time.deltaTime);
        }

        // Update circle direction randomly over time
        currentCircleSwitchTime += Time.deltaTime;
        if (currentCircleSwitchTime >= circleSwitchCooldown)
        {
            currentCircleSwitchTime = 0f;
            circleDirection = Random.value > 0.5f ? 1 : -1; // Randomly pick 1 or -1
        }

        // Circle around the player
        Vector3 circleDir = Quaternion.Euler(0, circleDirection * 90, 0) * targetDir; // Rotate the targetDir vector
        esm.cCon.Move(circleDir * agroSpeed * Time.deltaTime);

        if (distanceToTarget <= attackDistance && !attackOnCooldown)
        {
            AttackCheck();
        }
    }

    // Stationary behavior
    private void HandleStationary()
    {
        if (esm.eyes.distanceToTarget <= attackDistance && !attackOnCooldown)
        {
            AttackCheck();
        }
    }

    // Investigate behavior when losing sight of the target
    private void HandleInvestigate()
    {
        currentInvestigateTime += Time.deltaTime;

        if (currentInvestigateTime >= investigateTime)
        {
            esm.ChangeState(esm.patrolState);
        }
        else
        {
            currentInvestigateTime = 0;
        }
    }

    public void AttackCheck()
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
