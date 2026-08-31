using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfcrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explode Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private float growSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool canMoveToEnemy;

    [Header("Multi stacking crysta")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private float amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalList = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);
    }

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }

    #region Unlock skill region
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
        {
            crystalUnlocked = true;
            unlockCrystalButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockCrystalMirage() 
    {
        if (unlockCloneInsteadButton.unlocked)
        {
            cloneInsteadOfcrystal = true;
            unlockCloneInsteadButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
        {
            canExplode = true;
            unlockExplosiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
        {
            canMoveToEnemy = true;
            unlockMovingCrystalButton.GetComponent <Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockMultiStack()
    {
        if (unlockMultiStackButton.unlocked)
        {
            canUseMultiStacks = true;
            unlockMultiStackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if(SkillManager.instance.blackhole.currentBlackhole!=null )
            return;

        if(CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position= playerPos;

            if (cloneInsteadOfcrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCryStal();
            }
        }   
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, growSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalList.Count > 0)
            {
                if (crystalList.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);
                
                cooldown = 0;
                GameObject crystalToSpawn = crystalList[crystalList.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalList.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, growSpeed, FindClosestEnemy(newCrystal.transform), player);
                if(crystalList.Count<=0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            } 
        
        }
        return false;
    }

    private void RefilCrystal()
    {
        while(crystalList.Count<amountOfStacks)
        {
            crystalList.Add(crystalPrefab);    
        }
    }

    private void ResetAbility()
    {
        if(cooldownTimer>0)
            return;
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
