using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

[Serializable]
public struct Entry
{
#if UNITY_EDITOR
    public string KeyInternal;
#endif
    public uint Hash;
    public string Data;

    public static Entry New(uint hash)
    {
        return new Entry
        {
#if UNITY_EDITOR
            KeyInternal = string.Empty,
#endif
            Hash = hash,
            Data = string.Empty
        };
    }
}

struct EntryComparer : IComparer<Entry>
{
    public int Compare(Entry x, Entry y)
    {
        return x.Hash.CompareTo(y.Hash);
    }
}

[CreateAssetMenu(menuName = "Scriptable Objects/StaticTextContainer")]
[PreferBinarySerialization]
public class StaticTextContainer : ScriptableObject
{
    [SerializeField]
    private Entry[] entries;
#if UNITY_EDITOR
    private unsafe void OnValidate()
    {
        var keySpan = new Span<Entry>(entries);
        foreach (ref var k in keySpan)
        {
            k.KeyInternal = k.KeyInternal.ToLower();
            fixed (char* c = k.KeyInternal)
            {
                k.Hash = math.hash(c, k.KeyInternal.Length * sizeof(char));
            }
        }
    }
#endif

    public unsafe bool TryGetData(string key, out string value)
    {
        throw new NotImplementedException();
        // fixed (char* ptr = key)
        // {
        //     var hash = math.hash(ptr, sizeof(char) * key.Length);
        //     Array.BinarySearch(entries, new Entry {})
        //     var alias = internalKeys.CreateNativeArrayAlias();
        //     var idx = alias.BinarySearch(hash, );
        //     if (idx > -1)
        //     {
        //         value = data[idx];
        //         return true;
        //     }
        //     value = string.Empty;
        //     return false;
        // }
    }
}
