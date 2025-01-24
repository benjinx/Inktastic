using UnityEngine;

[System.Serializable]
public class EnemyPatrolState : EnemyState
{
    //rotate pointer around, move in distance

    public float newSearchInterval;
    public float searchReconDistance;
    public float patrolSpeed;

    private float currentSearchTime;
    private float currentSearchDistance;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        PickNewPatrolAngle();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        currentSearchTime += Time.deltaTime;
        currentSearchDistance += Time.deltaTime;

        if(currentSearchDistance <= searchReconDistance)
        {
            esm.cCon.Move(esm.pointerSprite.transform.forward * patrolSpeed * Time.deltaTime);
        }

        if(currentSearchTime >= newSearchInterval)
        {
            esm.currentLookAngle = PickNewPatrolAngle();
        }
    }


    public float PickNewPatrolAngle()
    {
        currentSearchDistance = 0;
        currentSearchTime = 0;
        return Random.Range(-180f, 180f);
    }
}
