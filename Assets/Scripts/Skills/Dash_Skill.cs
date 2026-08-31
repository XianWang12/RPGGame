using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked {  get; private set; }

    [Header("Clone on dash")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked {  get; private set; }

    [Header("Clone on arrive")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArriveUnlockButton;
    public bool cloneOnArrivalUnlocked {  get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();
        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArriveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    protected override void CheckUnlock()
    {
        UnlockCloneOnArrival();
        UnlockCloneOnDash();
        UnlockDash();

    }

    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;
            dashUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
        {
            cloneOnDashUnlocked = true;
            cloneOnDashUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockCloneOnArrival()
    {
        if (cloneOnArriveUnlockButton.unlocked)
        {
            cloneOnArrivalUnlocked = true;
            cloneOnArriveUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
}
