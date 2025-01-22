using UnityEngine;

[System.Serializable]
public class PlayerDodgeState : PlayerState
{
    public AnimationCurve velocityCurve;
    public float dodgeDuration;
    public float dodgeSpeedMultiplier;
    public float dodgeStartingSpeed;

    private float currentDodgeTime;
    private Vector3 dodgeDirection;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        //get current movement dir
        if(psm.controllerState.currentPlayerVelocity.magnitude >= 0.1)
        {
            dodgeDirection = psm.controllerState.currentPlayerVelocity.normalized;
        }
        else
        {
            //if zero use aim direction
            dodgeDirection = Quaternion.Euler(0, psm.controllerState.currentLookAngle, 0) * Vector3.forward;
        }

        //psm.controllerState.currentPlayerVelocity = dodgeDirection;
    }
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        currentDodgeTime += Time.deltaTime;

        psm.controllerState.cCon.Move((dodgeDirection * (velocityCurve.Evaluate(currentDodgeTime / dodgeDuration) + dodgeStartingSpeed) * dodgeSpeedMultiplier) * Time.deltaTime);

        if(currentDodgeTime >= dodgeDuration)
        {
            psm.ChangeState(psm.controllerState);
        }

    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        currentDodgeTime = 0;
    }
}
