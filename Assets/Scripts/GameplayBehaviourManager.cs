using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class GameplayBehaviourManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        var go = new GameObject(nameof(GameplayBehaviourManager))
        {
            hideFlags = HideFlags.NotEditable
        };
        DontDestroyOnLoad(go);
        go.AddComponent<GameplayBehaviourManager>();
    }

    private static readonly UpdateFlags[] Flags = Enum.GetValues(typeof(UpdateFlags)).Cast<UpdateFlags>().ToArray();
    private static readonly List<GameplayBehaviour> OnUpdates = new List<GameplayBehaviour>();
    private static readonly List<GameplayBehaviour> OnFixedUpdates = new List<GameplayBehaviour>();
    private static readonly List<GameplayBehaviour> OnLateUpdates = new List<GameplayBehaviour>();

    public static void Register(GameplayBehaviour behaviour)
    {
        foreach (var flag in Flags)
        {
            if ((flag & behaviour.UpdateFlags) > 0)
            {
                switch (flag)
                {
                    case UpdateFlags.Update:
                        OnUpdates.Add(behaviour);
                        break;
                    case UpdateFlags.LateUpdate:
                        OnLateUpdates.Add(behaviour);
                        break;
                    case UpdateFlags.FixedUpdate:
                        OnFixedUpdates.Add(behaviour);
                        break;
                }
            }
        }
    }

    public static void Unregister(GameplayBehaviour behaviour)
    {
        OnUpdates.Remove(behaviour);
        OnFixedUpdates.Remove(behaviour);
        OnLateUpdates.Remove(behaviour);
    }

    private void Awake()
    {
        OnUpdates.Clear();
        OnLateUpdates.Clear();
        OnFixedUpdates.Clear();
    }

    private void OnDestroy()
    {
        OnUpdates.Clear();
        OnLateUpdates.Clear();
        OnFixedUpdates.Clear();
    }

    private void Update() {
        foreach (var update in OnUpdates)
        {
            update.PerformUpdate();
        }
    }

    private void LateUpdate() {
        foreach (var update in OnLateUpdates)
        {
            update.PerformLateUpdate();
        }
    }

    private void FixedUpdate() {
        foreach (var update in OnFixedUpdates)
        {
            update.PerformFixedUpdate();
        }
    }
}
