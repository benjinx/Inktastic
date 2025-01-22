using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

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

    public static async Task LoadScenes(IReadOnlyList<int> toLoad, LoadSceneMode loadOptions = LoadSceneMode.Additive)
    {
        await LoadScenes(toLoad, null, loadOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task LoadScenes(IReadOnlyList<int> toLoad, Action onFinished, LoadSceneMode loadOptions = LoadSceneMode.Additive)
    {
        var tasks = new Task[toLoad.Count];
        var count = 0;
        foreach (var i in toLoad)
        {
            tasks[count++] = ConvertAsyncOperationToTask(SceneManager.LoadSceneAsync(i, loadOptions));
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }

    public static async Task UnloadScenes(IReadOnlyList<int> toUnload, UnloadSceneOptions unloadOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
    {
        await UnloadScenes(toUnload, null, unloadOptions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task UnloadScenes(IReadOnlyList<int> toUnload, Action onFinished, UnloadSceneOptions unloadOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
    {
        var tasks = new Task[toUnload.Count];
        var count = 0;
        foreach (var i in toUnload)
        {
            var op = SceneManager.UnloadSceneAsync(i, unloadOptions);
            if (op != null)
            {
                tasks[count++] = ConvertAsyncOperationToTask(op);
            }
        }
        await Task.WhenAll(tasks);
        onFinished?.Invoke();
    }
}