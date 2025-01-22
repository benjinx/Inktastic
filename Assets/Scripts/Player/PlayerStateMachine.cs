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
        controllerState.cCon = GetComponent<CharacterController>();
        dodgeState.psm = this;
        deathState.psm = this;

        ChangeState(controllerState);
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        FixedUpdateState();
    }

    private void LateUpdate()
    {
        LateUpdateState();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(controllerState.mouseWorldPosition, Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(controllerState.mouseScreenPosition, Vector3.one);
    }
}
