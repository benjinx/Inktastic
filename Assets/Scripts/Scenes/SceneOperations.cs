using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Threading;

public static class SceneOperations {
    public static Task ConvertAsyncOperationToTask(AsyncOperation asyncOperation)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        asyncOperation.completed += (operation) =>
        {
            if (asyncOperation.isDone)
            {
                taskCompletionSource.SetResult(true);
            }
            else
            {
                taskCompletionSource.SetException(new Exception("AsyncOperation failed."));
            }
        };
        return taskCompletionSource.Task;
    }

    public static async Task LoadScenes(SceneSet toLoad, Action onFinished) {
        var tasks = new Task[toLoad.SceneIndices.Count];
        var count = 0;
        foreach (var i in toLoad.SceneIndices) {
            tasks[count++] = ConvertAsyncOperationToTask(SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive));
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }

    public static async Task UnloadScenes(SceneSet toUnload, Action onFinished) {
        var tasks = new Task[toUnload.SceneIndices.Count];
        var count = 0;
        foreach (var i in toUnload.SceneIndices) {
            var op = SceneManager.UnloadSceneAsync(i, UnloadSceneOptions.None);
            Debug.Log(op == null);
            tasks[count++] = ConvertAsyncOperationToTask(op);
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }
}

class SceneOperationsManager : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeforeSceneLoad()
    {
        var go = new GameObject(nameof(SceneOperationsManager)) 
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        var _ = go.AddComponent<SceneOperationsManager>();
    }

    private void LateUpdate()
    {
    }
}