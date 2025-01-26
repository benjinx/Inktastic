using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneSet", menuName = "Scriptable Objects/SceneSet")]
public class SceneSet : ScriptableObject
{

    public IReadOnlyList<int> SceneLoadIndices => loadIndices;
    public IReadOnlyList<int> SceneUnloadIndices => unloadIndices;

    [SerializeField]
    private int[] loadIndices = Array.Empty<int>();
    [SerializeField]
    private int[] unloadIndices = Array.Empty<int>();

#if UNITY_EDITOR
    [SerializeField]
    [FormerlySerializedAs("sceneAssets")]
    private SceneAsset[] toLoad;

    [SerializeField]
    private SceneAsset[] toUnload;

    private void OnValidate()
    {
        Initialize(toLoad, ref loadIndices);
        Initialize(toUnload, ref unloadIndices);
    }

    private static void Initialize(SceneAsset[] sceneAssets, ref int[] indices)
    {
        var buildSettingScenes = EditorBuildSettings.scenes;
        indices = new int[sceneAssets.Length];

        var count = 0;
        for (var i = 0; i < buildSettingScenes.Length; i++)
        {
            EditorBuildSettingsScene s = buildSettingScenes[i];
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path);
            if (Array.IndexOf(sceneAssets, sceneAsset) > -1)
            {
                indices[count++] = i;
            }
        }
    }
#endif
}
