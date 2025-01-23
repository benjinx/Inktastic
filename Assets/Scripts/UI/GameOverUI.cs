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

    private UIDocument document;
    private VisualElement root;

    private void OnEnable()
    {
        GameplayStates.OnGameFinished += OnGameFinished;
        var debugAction = inputAsset.FindActionMap("Debug");
        debugAction.FindAction("EndGame").performed += EndGameContext;
    }

    private void OnDisable()
    {
        GameplayStates.OnGameFinished -= OnGameFinished;
        var debugAction = inputAsset.FindActionMap("Debug");
        debugAction.FindAction("EndGame").performed -= EndGameContext;
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

    private void EndGameContext(CallbackContext _)
    {
        GameplayStates.EndGame(Random.Range(0, 1) < 0.5f);
    }

    private void OnGameFinished(bool _)
    {
        root.style.display = DisplayStyle.Flex;
        var label = root.Q<Label>(UIIdentifiers.msg);
    }
}
