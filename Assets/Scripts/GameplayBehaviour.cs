using System;
using UnityEngine;

[Flags]
public enum UpdateFlags
{
    Update = 1 << 0,
    FixedUpdate = 1 << 1,
    LateUpdate = 1 << 2,
}

public abstract class GameplayBehaviour : MonoBehaviour
{
    public UpdateFlags UpdateFlags = (UpdateFlags)~0;

    public void PerformUpdate()
    {
        if ((UpdateFlags & UpdateFlags.Update) > 0)
        {
            OnUpdate();
        }
    }

    public void PerformFixedUpdate()
    {
        if ((UpdateFlags & UpdateFlags.FixedUpdate) > 0)
        {
            OnFixedUpdate();
        }
    }

    public void PerformLateUpdate()
    {
        if ((UpdateFlags & UpdateFlags.LateUpdate) > 0)
        {
            OnLateUpdate();
        }
    }

    protected virtual void OnEnable()
    {
        GameplayBehaviourManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        GameplayBehaviourManager.Unregister(this);
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnFixedUpdate()
    {
    }

    protected virtual void OnLateUpdate()
    {
    }
}
