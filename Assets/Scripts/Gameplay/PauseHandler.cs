using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PauseHandler : GameplayBehaviour
{
    [SerializeField]
    private InputActionAsset inputAsset;

    [NonSerialized]
    private bool isPaused = false;
    private void OnEnable()
    {
        GameplayStates.OnGamePaused += Pause;
        var map = inputAsset.FindActionMap("Game");
        var action = map.FindAction("PauseGame");
        action.performed += PauseInputHandler;
        map.Enable();
    }

    private void OnDisable()
    {
        GameplayStates.OnGamePaused -= Pause;
        var map = inputAsset.FindActionMap("Game");
        var action = map.FindAction("PauseGame");
        action.performed -= PauseInputHandler;
        map.Disable();
    }

    private void PauseInputHandler(CallbackContext _)
    {
        Pause();
    }

    private void Pause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            GameplayBehaviourManager.PauseAll();
        }
        else
        {
            GameplayBehaviourManager.ResumeAll();
        }
    }
}
