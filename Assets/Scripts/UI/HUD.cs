using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUD : GameplayBehaviour
{
    private VisualElement root;

    private VisualElement bossHealth;

    [SerializeField]
    private string bossName = "MOLUSK HELM";

#if UNITY_EDITOR
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("EDITOR ONLY, for debugging and testing features out.")]
    private float health = 0.5f;
#endif

    protected override void Awake()
    {
#if UNITY_EDITOR
        UpdateFlags = UpdateFlags.Update;
#else
        UpdateFlags = 0;
#endif
        base.Awake();
    }

    private void Start()
    {
        var document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        var boss = root.Q<TemplateContainer>("boss");
        bossHealth = boss.Q<VisualElement>("bar");
        boss.Q<Label>("name").text = bossName;
    }

    private void OnEnable()
    {
        GameplayStates.OnBossHealthDelta += OnBossHealthChange;
    }

    private void OnDisable()
    {
        GameplayStates.OnBossHealthDelta -= OnBossHealthChange;
    }

    private void OnBossHealthChange(float f)
    {
        bossHealth.style.width = new StyleLength(new Length(f * 100f, LengthUnit.Percent));
    }

#if UNITY_EDITOR
    protected override void OnUpdate()
    {
        bossHealth.style.width = new StyleLength(new Length(health * 100f, LengthUnit.Percent));
    }
#endif
}
