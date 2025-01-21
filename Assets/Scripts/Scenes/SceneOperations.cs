using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public static class SceneOperations
{
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

    public static async Task LoadScenes(SceneSet toLoad, LoadSceneMode loadOptions)
    {
        await LoadScenes(toLoad, null, loadOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task LoadScenes(SceneSet toLoad, Action onFinished, LoadSceneMode loadOptions = LoadSceneMode.Additive)
    {
        var tasks = new Task[toLoad.SceneIndices.Count];
        var count = 0;
        foreach (var i in toLoad.SceneIndices)
        {
            tasks[count++] = ConvertAsyncOperationToTask(SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive));
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }

    public static async Task UnloadScenes(SceneSet toUnload, UnloadSceneOptions unloadOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
    {
        await UnloadScenes(toUnload, null, unloadOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task UnloadScenes(SceneSet toUnload, Action onFinished, UnloadSceneOptions unloadOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
    {
        var tasks = new Task[toUnload.SceneIndices.Count];
        var count = 0;
        foreach (var i in toUnload.SceneIndices)
        {
            var op = SceneManager.UnloadSceneAsync(i, unloadOptions);
            Debug.Log(op == null);
            tasks[count++] = ConvertAsyncOperationToTask(op);
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }
}