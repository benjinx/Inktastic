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

    [Obsolete("Do not use if you want to disable the component's update, use the UpdateFlags instead.")]
    public new bool enabled {
        get => base.enabled;
        set => base.enabled = value;
    }

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

    protected virtual void Awake()
    {
        GameplayBehaviourManager.Register(this);
    }

    protected virtual void OnDestroy()
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
