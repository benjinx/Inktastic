using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerDeathState : PlayerState
{
    public UnityEvent onDeath;
    //play an explosion after a little bit, restart the scene

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        onDeath?.Invoke();
    }
}
