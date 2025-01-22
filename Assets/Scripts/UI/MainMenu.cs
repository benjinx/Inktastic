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
        var playBtn = document.rootVisualElement.Q<Button>(UIIdentifiers.play);
        playBtn.RegisterCallback<MouseUpEvent>(async _ =>
        {
            await SceneOperations.LoadScenes(sceneSet.SceneLoadIndices, () => Debug.Log("Finished load"));
            await SceneOperations.UnloadScenes(sceneSet.SceneUnloadIndices, () => Debug.Log("Finished unload"));
        });
    }
}
