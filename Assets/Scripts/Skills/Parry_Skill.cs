using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked {  get; private set; }
    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    public bool restoreUnlocked {  get; private set; }
    [Header("Parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked {  get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * .07f);
            player.stats.IncreaseHealtyBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();
        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryStore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryStore();
        UnlockParryWithMirage();

    }

    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
        {
            parryUnlocked = true;
            parryUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockParryStore()
    {
        if (parryUnlockButton.unlocked) 
        { 
            restoreUnlocked = true;
            parryUnlockButton.GetComponent <Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
        {
            parryWithMirageUnlocked=true;
            parryWithMirageUnlockButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
    }

    public void MakeMirageOnParry(Transform _respondTransform)
    {
        if(parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneOnCounterAttack(_respondTransform);
    }
}
