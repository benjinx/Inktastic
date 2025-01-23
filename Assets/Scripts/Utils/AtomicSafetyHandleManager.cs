#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

class AtomicSafetyHandleManager : MonoBehaviour
{
    private static List<AtomicSafetyHandle> Handles = new List<AtomicSafetyHandle>();

    internal static void Submit(AtomicSafetyHandle handle)
    {
        Handles.Add(handle);
    }

    internal static void Flush()
    {
        if (Handles.Count > 0)
        {
            Debug.Log($"Flushing {Handles.Count} AtomicSafetyHandles on frame: {Time.frameCount}");
        }

        foreach (var handle in Handles)
        {
            AtomicSafetyHandle.EnforceAllBufferJobsHaveCompletedAndRelease(handle);
        }
        Handles.Clear();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        var go = new GameObject(nameof(AtomicSafetyHandleManager))
        {
            hideFlags = HideFlags.NotEditable
        };
        go.AddComponent<AtomicSafetyHandleManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        var loop = PlayerLoop.GetCurrentPlayerLoop();

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            var subSystem = loop.subSystemList[i];

            if (subSystem.type == typeof(Initialization))
            {
                subSystem.updateDelegate -= Flush;
                subSystem.updateDelegate += Flush;
            }
        }
        PlayerLoop.SetPlayerLoop(loop);
    }
}

#endif