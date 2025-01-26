using UnityEngine;

[System.Serializable]
public class PlayerStunState : PlayerState
{
    public float stunDuration;

    private float currentStunTime;

    public override void OnStateEnter()
    {
        base.OnStateEnter();


    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        currentStunTime += Time.deltaTime;

        if(currentStunTime >= stunDuration)
        {
            psm.ChangeState(psm.controllerState);
        }
    }

    public override void OnStateExit()
    {
        base.OnStateExit();

        currentStunTime = 0;
    }
}
