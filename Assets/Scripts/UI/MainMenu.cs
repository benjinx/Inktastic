using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    private UIDocument document;

    [SerializeField]
    private SceneSet toLoad;

    [SerializeField]
    private SceneSet toUnload;

    private void Start()
    {
        this.document = GetComponent<UIDocument>();
        var playBtn = document.rootVisualElement.Q<Button>(UIIdentifiers.play);
        playBtn.RegisterCallback<MouseUpEvent>(async _ => 
        {
            await SceneOperations.LoadScenes(toLoad, () => Debug.Log("Finished load"));
            await SceneOperations.UnloadScenes(toUnload, () => Debug.Log("Finished unload"));
        });
    }
}
