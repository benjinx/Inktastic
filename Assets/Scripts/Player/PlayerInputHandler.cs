using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInputHandler : GameplayBehaviour
{
    public PlayerStateMachine psm;
    public bool gamePad;

    [HideInInspector]
    public InputMain controls;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction dodgeAction;
    private InputAction attackAction;


    protected override void Awake()
    {
        base.Awake();
        psm = GetComponent<PlayerStateMachine>();
    }

    public void Start()
    {
        controls = new InputMain();
        moveAction = controls.Player.Move;
        lookAction = controls.Player.Look;
        dodgeAction = controls.Player.Dodge;
        attackAction = controls.Player.Attack;
        moveAction.Enable();
        lookAction.Enable();
        dodgeAction.Enable();
        attackAction.Enable();
        InputSystem.onEvent += OnInputEvent;
        dodgeAction.performed += HandleDodgeInput;
        attackAction.performed += HandleAttackInput;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Check if the device is currently sending input
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

        // Determine the type of device
        if (device is Gamepad)
        {
            gamePad = true;
        }
        else if (device is Keyboard || device is Mouse)
        {
            gamePad = false;
        }

    }

    public void HandleDodgeInput(InputAction.CallbackContext obj)
    {
        if (obj.performed && psm.GetCurrentState() != psm.dodgeState && psm.GetCurrentState() != psm.attackState)
        {
            psm.TryChangeState(psm.dodgeState);
        }
    }

    public void HandleAttackInput(InputAction.CallbackContext obj)
    {
        if (obj.performed && psm.GetCurrentState() != psm.attackState && psm.GetCurrentState() != psm.dodgeState)
        {
            psm.TryChangeState(psm.attackState);
        }
    }

    protected override void OnUpdate()
    {
        psm.controllerState.UpdatePlayerMovement(moveAction.ReadValue<Vector2>().normalized);

        if (gamePad)
        {
            psm.controllerState.UpdatePlayerLook(lookAction.ReadValue<Vector2>().normalized);
        }
        else
        {
            psm.controllerState.UpdatePlayerLook(Mouse.current.position.ReadValue());
        }
    }

}