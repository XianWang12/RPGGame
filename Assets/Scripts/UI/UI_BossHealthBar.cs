using UnityEngine;
using UnityEngine.UI;

public class UI_BossHealthBar : MonoBehaviour
{
    [SerializeField] private Enemy_Boss boss;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject healthBarRoot;

    private CharacterStats bossStats;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (healthBarRoot == null)
            healthBarRoot = gameObject;

        if (slider == null)
            slider = GetComponentInChildren<Slider>(true);

        if (boss == null)
            boss = FindObjectOfType<Enemy_Boss>();

        canvasGroup = healthBarRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = healthBarRoot.AddComponent<CanvasGroup>();

        if (!healthBarRoot.activeSelf)
            healthBarRoot.SetActive(true);
    }

    private void OnEnable()
    {
        TryBindBossStats();

        if (bossStats != null)
            bossStats.onHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (bossStats != null)
            bossStats.onHealthChanged -= UpdateHealthUI;
    }

    private void Start()
    {
        TryBindBossStats();
        UpdateHealthUI();
        RefreshVisibility();
    }

    private void Update()
    {
        if (boss == null || bossStats == null)
            TryBindBossStats();

        RefreshVisibility();
    }

    private void TryBindBossStats()
    {
        if (boss == null)
            boss = FindObjectOfType<Enemy_Boss>();

        CharacterStats newStats = boss != null ? boss.stats : null;
        if (newStats == bossStats)
            return;

        if (bossStats != null)
            bossStats.onHealthChanged -= UpdateHealthUI;

        bossStats = newStats;

        if (bossStats != null && isActiveAndEnabled)
            bossStats.onHealthChanged += UpdateHealthUI;
    }

    private void RefreshVisibility()
    {
        if (canvasGroup == null)
            return;

        bool shouldShow = boss != null && bossStats != null && boss.showBossHealthBar() && !bossStats.isDie;

        canvasGroup.alpha = shouldShow ? 1 : 0;
        canvasGroup.interactable = shouldShow;
        canvasGroup.blocksRaycasts = shouldShow;
    }

    private void UpdateHealthUI()
    {
        if (slider == null || bossStats == null)
            return;

        slider.maxValue = bossStats.GetMaxHealthValue();
        slider.value = bossStats.currentHealth;
    }
}
