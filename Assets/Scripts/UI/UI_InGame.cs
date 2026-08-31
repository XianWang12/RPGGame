using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    [SerializeField] private TextMeshProUGUI currency;

    private SkillManager skills;

    private void Start()
    {
        skills = SkillManager.instance;

        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

    }

    private void Update()
    {
        currency.text = PlayerManager.instance.GetCurrency().ToString("#,#");

        if (Input.GetKeyDown(KeyCode.LeftShift)&&skills.dash.dashUnlocked)
            SetCooldown(dashImage);
        if(Input.GetKeyDown(KeyCode.Q)&&skills.parry.parryUnlocked)
            SetCooldown(parryImage);
        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldown(crystalImage);
        if(Input.GetKeyDown(KeyCode.Mouse1)&&skills.sword.swordUnlocked)
            SetCooldown(swordImage);
        if(Input.GetKeyDown(KeyCode.R)&&skills.blackhole.blackholeUnlocked)
            SetCooldown(blackholeImage);
        if(Input.GetKeyDown(KeyCode.E)&&Inventory.Instance.GetEquipment(EquipmentType.Flask)!=null)
            SetCooldown(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(flaskImage, 21);
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldown(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
