using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/StaticTextContainer")]
[PreferBinarySerialization]
public class StaticTextContainer : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField]
    private string[] keys = Array.Empty<string>();
#endif
    [SerializeField]
    private string[] data = Array.Empty<string>();

    private uint[] internalKeys = Array.Empty<uint>();

#if UNITY_EDITOR
    private unsafe void OnValidate()
    {
        var keySpan = new Span<string>(keys);
        foreach (ref var k in keySpan) {
            k = k.ToLower();
        }
    }
#endif

    public unsafe bool TryGetData(string key, out string value)
    {
        fixed (char* ptr = key)
        {
            var hash = math.hash(ptr, sizeof(char) * key.Length);
            var alias = internalKeys.CreateNativeArrayAlias();
            var idx = alias.BinarySearch(hash);
            if (idx > -1)
            {
                value = data[idx];
                return true;
            }
            value = string.Empty;
            return false;
        }

    }
}
