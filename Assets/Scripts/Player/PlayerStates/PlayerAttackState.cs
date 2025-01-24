using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerAttackState : PlayerDodgeState
{
    public Hitbox attackHitbox;
    public GameObject attackParent;
    public float attackDuration;
    public float damage;
    public UnityEvent onAttack;

    public override void OnStateEnter()
    {
        onAttack?.Invoke();
        attackHitbox.ActivateHitbox(attackDuration, damage);

        if (psm.controllerState.currentPlayerVelocity.magnitude >= 0.1)
        {
            dashDirection = psm.controllerState.currentPlayerVelocity.normalized;
            psm.controllerState.currentLookAngle = Mathf.Atan2(dashDirection.x, dashDirection.z);
        }
        else
        {
            //if zero use aim direction
            dashDirection = Quaternion.Euler(0, psm.controllerState.currentLookAngle, 0) * Vector3.forward;
        }

        attackParent.transform.forward = dashDirection;
        //spearSprite.SetActive(true);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        //spearSprite.SetActive(false);   
    }
}
