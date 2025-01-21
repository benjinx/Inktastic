using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;




#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneSet", menuName = "Scriptable Objects/SceneSet")]
public class SceneSet : ScriptableObject
{
    public IReadOnlyList<int> SceneIndices => sceneIndices;
    private int[] sceneIndices = new int[0];

#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset[] sceneAssets;

    private void OnValidate()
    {
        Initialize();
    }

    private void Initialize()
    {
        var scenes = EditorBuildSettings.scenes;
        sceneIndices = new int[sceneAssets.Length];

        var count = 0;
        for (var i = 0; i < scenes.Length; i++)
        {
            EditorBuildSettingsScene s = scenes[i];
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path);
            if (Array.IndexOf(sceneAssets, sceneAsset) > -1)
            {
                Debug.Log($"Found {sceneAsset} at build index {i}");
                sceneIndices[count++] = i;
            }
        }
    }
#endif
}
