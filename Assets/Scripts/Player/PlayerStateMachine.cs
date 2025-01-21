using UnityEngine;
using BizarreTools.StateMachine;

public class PlayerStateMachine : StateMachine
{
    public PlayerControllerState controllerState;
    public PlayerDodgeState dodgeState;
    public PlayerDeathState deathState;

    private void Start()
    {
        SetupStateMachine();
    }

    public void SetupStateMachine()
    {
        controllerState.psm = this;
        dodgeState.psm = this;
        deathState.psm = this;

        ChangeState(controllerState);
    }
}
