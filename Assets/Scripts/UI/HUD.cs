using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct ProgressBarBindings
{
    public VisualElement ProgressBar;
    public Label Fraction;
#if UNITY_EDITOR
    [Header("Editor Only Debugging")]
    [Tooltip("Editor only, used for testing")]
    [Range(0, 100)]
    public int RawValue;

    [Tooltip("Editor only, used for testing")]
    [Range(2, 100)]
    public int Denominator;
#endif

    public ProgressBarBindings(VisualElement progressBar, Label fraction)
    {
#if UNITY_EDITOR
        RawValue = 0;
        Denominator = 100;
#endif
        ProgressBar = progressBar;
        Fraction = fraction;
    }

    public void Update(int rawValue, int denominator)
    {
        var norm = Mathf.Clamp01((float)rawValue / denominator);
        ProgressBar.style.width = new StyleLength(new Length(norm * 100f, LengthUnit.Percent));
        Fraction.text = $"{rawValue} / {denominator}";
    }
}

[RequireComponent(typeof(UIDocument))]
public class HUD : GameplayBehaviour
{
    private VisualElement root;

    private VisualElement bossHealth;

    [SerializeField]
    private string bossName = "MOLUSK HELM";

    [SerializeField]
    private ProgressBarBindings playerHealthBinding;
    [SerializeField]
    private ProgressBarBindings ammoBinding;

#if UNITY_EDITOR
    [Header("Editor Only Debugging")]
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

        var playerHealthGroup = root.Q<VisualElement>("player-health");
        playerHealthBinding = new ProgressBarBindings(playerHealthGroup.Q<VisualElement>("bar"), playerHealthGroup.Q<Label>("fraction"));

        var ammoGroup = root.Q<VisualElement>("ammo");
        ammoBinding = new ProgressBarBindings(ammoGroup.Q<VisualElement>("bar"), ammoGroup.Q<Label>("fraction"));
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
        ammoBinding.Update(ammoBinding.RawValue, ammoBinding.Denominator);
        playerHealthBinding.Update(playerHealthBinding.RawValue, playerHealthBinding.Denominator);
    }
#endif
}
