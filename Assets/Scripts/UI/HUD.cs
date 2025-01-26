// #define DEBUG
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
#if UNITY_EDITOR && DEBUG
       // UpdateFlags = UpdateFlags.Update;
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

        boss.visible = false;

        var playerHealthGroup = root.Q<VisualElement>("player-health");
        playerHealthBinding = new ProgressBarBindings(playerHealthGroup.Q<VisualElement>("bar"), playerHealthGroup.Q<Label>("fraction"));

        var ammoGroup = root.Q<VisualElement>("ammo");
        ammoBinding = new ProgressBarBindings(ammoGroup.Q<VisualElement>("bar"), ammoGroup.Q<Label>("fraction"));
    }

    public void EnableBossUI()
    {
        var boss = root.Q<TemplateContainer>("boss");

        boss.visible = true;
    }

    private void OnEnable()
    {
        GameplayStates.OnBossHealthDelta += OnBossHealthChange;
        GameplayStates.OnPlayerAmmoDelta += OnPlayerAmmoChange;
        GameplayStates.OnPlayerHealthDelta += OnPlayerHealthChange;

    }

    private void OnDisable()
    {
        GameplayStates.OnBossHealthDelta -= OnBossHealthChange;
        GameplayStates.OnPlayerAmmoDelta -= OnPlayerAmmoChange;
        GameplayStates.OnPlayerHealthDelta -= OnPlayerHealthChange;
    }

    private void OnBossHealthChange(float f)
    {
        bossHealth.style.width = new StyleLength(new Length(f * 100f, LengthUnit.Percent));
    }

    private void OnPlayerAmmoChange(int rawValue, int denominator)
    {
        ammoBinding.Update(rawValue, denominator);
    }

    private void OnPlayerHealthChange(int rawValue, int denominator)
    {
        playerHealthBinding.Update(rawValue, denominator);
    }

#if UNITY_EDITOR && DEBUG
    protected override void OnUpdate()
    {
        GameplayStates.ChangeBossHealth(health);
        GameplayStates.ChangePlayerHealth(playerHealthBinding.RawValue, playerHealthBinding.Denominator);
        GameplayStates.ChangePlayerAmmo(ammoBinding.RawValue, ammoBinding.Denominator);
    }
#endif
}
