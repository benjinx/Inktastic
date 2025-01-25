using UnityEngine;
using BizarreTools.StateMachine;

public class EnemyStateMachine : BizarreTools.StateMachine.StateMachine
{
    public EnemyPatrolState patrolState;
    public EnemyAgroState agroState;
    public EnemyAttackState attackState;
    public EnemyStunState stunState;
    public EnemyDeathState deathState;

    public SpriteRenderer actorSprite;

    public EnemyEyes eyes;
    public float currentLookAngle;
    public GameObject pointerSprite;
    public CharacterController cCon;

    private void Start()
    {
        cCon = GetComponent<CharacterController>(); 
        SetupStateMachine();
    }

    public void SetupStateMachine()
    {
        patrolState.esm = this;
        agroState.esm = this;
        attackState.esm = this;
        stunState.esm = this;
        deathState.esm = this;

        DefineTransition(agroState, attackState);
        DefineTransition(attackState, agroState);

        ChangeState(patrolState);
    }

    public void HandleSpriteDirection()
    {
        actorSprite.flipX = currentLookAngle <= 0;
    }

    public void OnSpook()
    {
        ChangeState(agroState);
    }

    protected override void OnUpdate()
    {
        UpdateState();

        pointerSprite.transform.rotation = Quaternion.Euler(0, currentLookAngle, 0);

        HandleSpriteDirection();
    }

}
