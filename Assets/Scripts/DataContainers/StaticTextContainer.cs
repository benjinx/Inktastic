using System;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public struct Entry
{
#if UNITY_EDITOR
    [SerializeField]
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

public struct EntryComparer : IComparer<Entry>
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
    [FormerlySerializedAs("entries")]
    public Entry[] Entries;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Sort()
    {
        foreach (var staticTextContainer in Resources.FindObjectsOfTypeAll<StaticTextContainer>())
        {
            Array.Sort(staticTextContainer.Entries, default(EntryComparer));
        }
    }

#if UNITY_EDITOR
    private unsafe void OnValidate()
    {
        var keySpan = new Span<Entry>(Entries);
        foreach (ref var k in keySpan)
        {
            k.KeyInternal = k.KeyInternal.Trim().ToLower();
            fixed (char* c = k.KeyInternal)
            {
                k.Hash = math.hash(c, k.KeyInternal.Length * sizeof(char));
            }
        }
    }
#endif

    public unsafe bool TryGetData(string key, out string value)
    {
        fixed (char* ptr = key)
        {
            var hash = math.hash(ptr, sizeof(char) * key.Length);
            var idx = Array.BinarySearch(Entries, new Entry { Hash = hash }, default(EntryComparer));
            if (idx > -1)
            {
                value = Entries[idx].Data;
                return true;
            }
            value = string.Empty;
            return false;
        }
    }
}
