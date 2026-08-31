using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private float bounceGravity;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCoolDown = .35f;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float maxTracelDistance = 12;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked {  get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get;private set; }
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked { get;private set; }

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceOfDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        GeneralDots();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
    }

    private void SetUpGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
            for (int i = 0; i < dots.Length; i++)
                dots[i].transform.position = DotsPosition(i * spaceOfDots);
    }

    public void CreateSword()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = cooldown;

        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);

        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        switch (swordType)
        {
            case SwordType.Bounce:
                newSwordScript.SetUpBounce(true, bounceAmount, bounceSpeed);
                break;
            case SwordType.Pierce:
                newSwordScript.SetUpPierce(pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetUpSpin(true, maxTracelDistance, spinDuration, hitCoolDown);
                break;
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
        UnlockTimeStop();
        UnlockVulnerable();

    }

    #region Unlock region
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
        {
            timeStopUnlocked = true;
            timeStopUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockVulnerable()
    {
        if (vulnerableUnlockButton.unlocked)
        {
            vulnerableUnlocked = true;
            vulnerableUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
            swordUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Bounce;
            SetUpGravity();
            bounceUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
        {
            swordType = SwordType.Pierce;
            SetUpGravity();
            pierceUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
        {
            swordType = SwordType.Spin;
            SetUpGravity();
            spinUnlockButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;
        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
            dots[i].SetActive(_isActive);
    }

    private void GeneralDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }
    #endregion
}
