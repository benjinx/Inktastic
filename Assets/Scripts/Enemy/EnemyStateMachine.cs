using UnityEngine;
using BizarreTools.StateMachine;

public class EnemyStateMachine : BizarreTools.StateMachine.StateMachine
{
    public EnemyPatrolState patrolState;
    public EnemyAgroState agroState;
    public EnemyAttackState attackState;
    public EnemyStunState stunState;


}
