using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(UIDocument))]
public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private SceneSet onLoseSceneSet;

    [SerializeField]
    private bool visible;

    [SerializeField]
    private InputActionAsset inputAsset;

    [Tooltip("The first element must always be the lose message as false is considered 0.")]
    [SerializeField]
    private string[] messages = new string[2];

    [SerializeField]
    private Texture2D[] backgrounds = new Texture2D[2];

    private UIDocument document;
    private VisualElement root;

    private static void AssertArray<T>(ref T[] src, int size = 2)
    {
        if (src.Length != size)
        {
            var array = new T[size];
            for (var i = 0; i < size; i++)
            {
                array[i] = src[i];
            }
            // Replace the src with the copy.
            src = array;
        }
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

    private void OnValidate()
    {
        AssertArray(ref messages);
        AssertArray(ref backgrounds);
    }

    private void OnEnable()
    {
        GameplayStates.OnGameFinished += OnGameFinished;
        var debugAction = inputAsset.FindActionMap("Debug");
#if UNITY_EDITOR || DEBUG
        debugAction.Enable();
        debugAction.FindAction("EndGame").performed += EndGameContext;
#else
        debugAction.Disable();
#endif
    }

    private void OnDisable()
    {
        GameplayStates.OnGameFinished -= OnGameFinished;
#if UNITY_EDITOR || DEBUG
        var debugAction = inputAsset.FindActionMap("Debug");
        debugAction.FindAction("EndGame").performed -= EndGameContext;
#endif
    }

    private void Start()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        var returnGroup = root.Q<TemplateContainer>("return");
        ProcessButtonGroup(root, "return", (img, btn) =>
        {
            btn.text = "RETURN";
            AttachCommonCallbacks(btn, img);
            AttachCommonCallbacks(img, img);
            img.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });

            btn.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });
        });

        ProcessButtonGroup(root, "retry", (img, btn) =>
        {
            btn.text = "TRY AGAIN";
            AttachCommonCallbacks(btn, img);
            AttachCommonCallbacks(img, img);
            img.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });

            btn.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });
        });
    }

#if UNITY_EDITOR || DEBUG
    private void EndGameContext(CallbackContext _)
    {
        var win = Random.Range(0f, 1f) < 0.5f;
        GameplayStates.EndGame(win);
    }
#endif

    private void OnGameFinished(bool win)
    {
        int idx = AsInt(win);
        var btnTextColor = win ? Color.black : Color.white;
        root.style.display = DisplayStyle.Flex;
        var label = root.Q<Label>(UIIdentifiers.msg);
        label.text = messages[idx];

        var background = root.Q<VisualElement>("background");
        background.style.backgroundImage = backgrounds[idx];

        ProcessButtonGroup(root, "retry", (img, btn) =>
        {
            btn.style.color = btnTextColor;
            btn.text = "TRY AGAIN";

            img.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });

            btn.RegisterCallback<MouseUpEvent>(async _ =>
            {
                await SceneOperations.LoadScenes(onLoseSceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
                await SceneOperations.UnloadScenes(onLoseSceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
                root.style.display = DisplayStyle.None;
            });
        });

        ProcessButtonGroup(root, "return", (img, btn) =>
        {
            btn.style.color = btnTextColor;
            btn.text = "QUIT";

            img.RegisterCallback<MouseUpEvent>(_ =>
            {
                root.style.display = DisplayStyle.None;
                Application.Quit();
            });

            btn.RegisterCallback<MouseUpEvent>(_ =>
            {
                root.style.display = DisplayStyle.None;
                Application.Quit();
            });
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int AsInt(bool value)
    {
        return value ? 1 : 0;
    }
}
