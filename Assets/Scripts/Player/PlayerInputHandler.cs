using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerStateMachine psm;
    public bool gamePad;

    [HideInInspector]
    public InputMain controls;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction dodgeAction;


    private void Awake()
    {
        psm = GetComponent<PlayerStateMachine>();
    }

    public void Start()
    {
        controls = new InputMain();
        moveAction = controls.Player.Move;
        lookAction = controls.Player.Look;
        dodgeAction = controls.Player.Dodge;
        moveAction.Enable();
        lookAction.Enable();
        dodgeAction.Enable();
        InputSystem.onEvent += OnInputEvent;
        dodgeAction.performed += HandleDodgeInput;

    }

    private void OnDestroy()
    {
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
        if (obj.performed && psm.GetCurrentState() != psm.dodgeState)
        {
            psm.ChangeState(psm.dodgeState);
        }
    }

    private void Update()
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