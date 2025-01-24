using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(UIDocument))]
public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private SceneSet sceneSet;

    [SerializeField]
    private bool visible;

    [SerializeField]
    private InputActionAsset inputAsset;

    [Tooltip("The first element must always be the lose message as false is considered 0.")]
    [SerializeField]
    private string[] messages = new string[2];

    private UIDocument document;
    private VisualElement root;

    private void OnValidate()
    {
        if (messages.Length != 2)
        {
            var array = new string[2];
            for (int i = 0; i < 2; i++)
            {
                array[i] = messages[i];
            }
            messages = array;
        }
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

        var returnBtn = root.Q<Button>(UIIdentifiers.@return);
        returnBtn.RegisterCallback<MouseUpEvent>(async _ =>
        {
            await SceneOperations.LoadScenes(sceneSet.SceneLoadIndices, () => Debug.Log("Loading finished"));
            await SceneOperations.UnloadScenes(sceneSet.SceneUnloadIndices, () => Debug.Log("Unloading finished"));
            root.style.display = DisplayStyle.None;
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
        root.style.display = DisplayStyle.Flex;
        var label = root.Q<Label>(UIIdentifiers.msg);
        label.text = win ? messages[1] : messages[0];
    }
}
