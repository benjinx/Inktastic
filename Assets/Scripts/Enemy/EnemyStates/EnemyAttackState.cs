using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    public Hitbox attackHitbox;
    public GameObject attackParent;
    public float attackDuration;
    public float damage;
    public UnityEvent onAttack;

    private float currentAttackDuration;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        onAttack?.Invoke();
        attackHitbox.ActivateHitbox(attackDuration, damage);

       
        attackParent.transform.forward = Quaternion.Euler(0, esm.currentLookAngle, 0) * Vector3.forward;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        currentAttackDuration += Time.deltaTime;

        if(currentAttackDuration >= attackDuration)
        {
            esm.ChangeState(esm.agroState);
        }
    }

    public override void OnStateExit()
    {
        base.OnStateExit();

        currentAttackDuration = 0;
    }
}
