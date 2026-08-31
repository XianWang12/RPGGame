using UnityEngine;

public class Enemy_Boss : Enemy
{
    #region States
    public BossBattleState battleState { get; private set; }
    public BossMoveState moveState { get; private set; }
    public BossSpellCastState spellCastState { get; private set; }
    public BossTeleportState teleportState { get; private set; }
    public BossDeadState deadState { get; private set; }
    public BossIdleState idleState { get; private set; }
    public BossAttackState attackState { get; private set; }
    #endregion

    public bool bossFightStarted;

    [Header("Spell Cast details")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    [SerializeField] private float spellStateCooldown;

    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    private float defaultChanceToTeleport = 40;

    [Header("Visual")]
    [SerializeField] private Transform visualRoot;

    protected override void Awake()
    {
        base.Awake();

        battleState = new BossBattleState(this, stateMachine, "Move", this);
        moveState = new BossMoveState(this, stateMachine, "Move", this);
        spellCastState = new BossSpellCastState(this, stateMachine, "SpellCast", this);
        teleportState = new BossTeleportState(this, stateMachine, "Teleport", this);
        deadState = new BossDeadState(this, stateMachine, "Idle", this);
        idleState = new BossIdleState(this, stateMachine, "Idle", this);
        attackState = new BossAttackState(this, stateMachine, "Attack", this);
    }

    protected override void Start()
    {
        base.Start();

        Transform targetVisualRoot = visualRoot != null ? visualRoot : anim != null ? anim.transform : null;
        if (targetVisualRoot != null)
        {
            Vector3 scale = targetVisualRoot.localScale;
            scale.x *= -1;
            targetVisualRoot.localScale = scale;
        }

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        
    }

    public bool showBossHealthBar()
    {
        if(Vector2.Distance(PlayerManager.instance.player.transform.position, transform.position) < 21)
            return true;
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;
        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * 3;

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + 2.1f);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<BossSpell_Controller>().SetUpSpell(stats);
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    private bool SomethingIsGround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

        if (!GroundBelow() || SomethingIsGround())
        {
            Debug.Log("find new position");
            FindPosition();
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }
        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time > lastTimeCast + spellStateCooldown)
        {
            return true;
        }
        return false;
    }
}
