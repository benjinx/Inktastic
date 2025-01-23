using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerDeathState : PlayerState
{
    public UnityEvent onDeath;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        onDeath?.Invoke();
    }
}
