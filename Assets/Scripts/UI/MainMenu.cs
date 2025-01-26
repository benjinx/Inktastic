using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    private UIDocument document;

    [SerializeField]
    [FormerlySerializedAs("toLoad")]
    private SceneSet sceneSet;

    private void Start()
    {
        this.document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        ProcessButtonGroup(root, "play-group", (img, btn) =>
        {
            btn.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(sceneSet.SceneLoadIndices, () =>
                {
                    foreach (var item in sceneSet.SceneLoadIndices)
                    {
                        Debug.Log("LOADED " + item);
                    }
                });

          

               
                await SceneOperations.UnloadScenes(sceneSet.SceneUnloadIndices, () =>
                {
                    foreach (var item in sceneSet.SceneUnloadIndices)
                    {
                        Debug.Log("UNLOADED " + item);
                    }
                });
            });

            img.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(sceneSet.SceneLoadIndices, () => Debug.Log("Finished load"));
                await SceneOperations.UnloadScenes(sceneSet.SceneUnloadIndices, () => Debug.Log("Finished unload"));
            });

            AttachCommonCallbacks(img, img);
            AttachCommonCallbacks(btn, img);
        });

        ProcessButtonGroup(root, "credit-group", (img, btn) =>
        {
            btn.text = "Credits";
            AttachCommonCallbacks(img, img);
            AttachCommonCallbacks(btn, img);
        });

        ProcessButtonGroup(root, "quit-group", (img, btn) =>
        {
            AttachCommonCallbacks(img, img);
            AttachCommonCallbacks(btn, img);
            btn.text = "Quit";
            btn.RegisterCallback<MouseUpEvent>(_ =>
            {
                Application.Quit();
            });
        });
    }

    private static void AttachCommonCallbacks(VisualElement element, VisualElement target)
    {
        element.RegisterCallback<MouseEnterEvent>(_ =>
        {
            target.RemoveFromClassList("invisible");
        });

        element.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            target.AddToClassList("invisible");
        });
    }

    private static void ProcessButtonGroup(VisualElement root, string id, Action<VisualElement, Button> action)
    {
        var container = root.Q<TemplateContainer>(id);
        var ptr = container.Q<VisualElement>("pointer");
        var btn = container.Q<Button>("btn");
        action?.Invoke(ptr, btn);
    }
}
