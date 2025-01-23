using UnityEngine;

[System.Serializable]
public class PlayerDodgeState : PlayerState
{
    public AnimationCurve velocityCurve;
    public float dashDuration;
    public float dashSpeedMultiplier;
    public float dashStartingSpeed;

    protected float currentDashTime;
    protected Vector3 dashDirection;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        //get current movement dir
        if(psm.controllerState.currentPlayerVelocity.magnitude >= 0.1)
        {
            dashDirection = psm.controllerState.currentPlayerVelocity.normalized;
        }
        else
        {
            //if zero use aim direction
            dashDirection = Quaternion.Euler(0, psm.controllerState.currentLookAngle, 0) * Vector3.forward;
        }

        //psm.controllerState.currentPlayerVelocity = dodgeDirection;
    }
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        currentDashTime += Time.deltaTime;

        psm.controllerState.cCon.Move((dashDirection * (velocityCurve.Evaluate(currentDashTime / dashDuration) + dashStartingSpeed) * dashSpeedMultiplier) * Time.deltaTime);

        if(currentDashTime >= dashDuration)
        {
            psm.ChangeState(psm.controllerState);
        }

    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        currentDashTime = 0;
    }
}
