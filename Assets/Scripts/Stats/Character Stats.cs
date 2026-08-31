using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    vitality,
    agility,
    intelligence,
    damage,
    criChance,
    critPower,
    maxHealth,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightingDamage,
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;


    [Header("Major stats")]
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    public Stat vitality;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;


    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    [Space]
    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDie { get; private set; }
    private bool isVulnerable;

    private void Awake()
    {
        critPower.SetDefautValue(150);
    }

    protected virtual void Start()
    {
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;
        if (isIgnited)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration)=>StartCoroutine(VulnerableForCoroutine(_duration));

    private IEnumerator VulnerableForCoroutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }
    public virtual void DoDamage(CharacterStats _targetStat)
    {
        if (TargetCanAvoidAttack(_targetStat))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStat, totalDamage);

        _targetStat.TakeDamage(totalDamage, transform);
        DoMagicalDamage(_targetStat);
    }


    #region Magical damage and ailments
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        //
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage, transform);

        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        if (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            int temp = Random.Range(0, 3);
            switch (temp)
            {
                case 0: canApplyIgnite = true; break;
                case 1: canApplyChill = true; break;
                case 2: canApplyShock = true; break;
                default: canApplyIgnite = true; break;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .15f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .15f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyshock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite)
        {
            ignitedTimer = ailmentsDuration;
            isIgnited = _ignite;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            isChilled = _chill;

            GetComponent<Entity>().SlowEntityBy(.2f, ailmentsDuration);

            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyshock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.transform == transform)
                continue;
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDie)
            {
                isDie = true;
                Die();
            }

            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion


    public virtual void TakeDamage(int _damage)
    {
        TakeDamage(_damage, null);
    }

    public virtual void TakeDamage(int _damage, Transform _attacker)
    {
        if (isVulnerable)
            _damage  = Mathf.RoundToInt( _damage*1.2f);

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact(_attacker);

        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDie)
        {
            isDie = true;
            Die();
        }
    }

    public virtual void IncreaseHealtyBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {

    }


    #region Stat calculations
    protected int CheckTargetArmor(CharacterStats _targetStat, int totalDamage)
    {
        if (isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStat.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStat.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStat, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStat.magicResistance.GetValue() + (_targetStat.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStat)
    {
        int totalEvasion = _targetStat.evasion.GetValue() + _targetStat.agility.GetValue();

        if (isChilled)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStat.OnEvasion();
            return true;
        }
        return false;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < totalCriticalChance)
            return true;
        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = totalCritPower * _damage;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    #endregion

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength:
                return strength;
            case StatType.vitality:
                return vitality;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.damage:
                return damage;
            case StatType.criChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.lightingDamage:
                return lightingDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.maxHealth:
                return maxHealth;
            case StatType.magicResistance:
                return magicResistance;
            default:
                return null;
        }
    }
}
