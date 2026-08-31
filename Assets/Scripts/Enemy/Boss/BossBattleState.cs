using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleState : EnemyState
{
    private int moveDir;
    private Enemy_Boss enemy;
    private Transform Player;
    public BossBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Boss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.battleTime;
        Player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        float distanceToPlayer = Vector2.Distance(Player.position, enemy.transform.position);
        if (distanceToPlayer < enemy.attackDistance && CanAttack())
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
            return;
        }

        float xDistance = Mathf.Abs(Player.position.x - enemy.transform.position.x);
        float yDistance = Mathf.Abs(Player.position.y - enemy.transform.position.y);
        float overlapDeadZone = .2f;

        if (xDistance <= overlapDeadZone && yDistance <= 1f)
        {
            enemy.SetZeroVelocity();
            return;
        }

        if (Player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (Player.position.x < enemy.transform.position.x)
            moveDir = -1;

        enemy.SetVelocity(moveDir * enemy.moveSpeed * 1.2f, rb.velocity.y);
    }
    public bool CanAttack()
    {
        if (Time.time > enemy.lastTimeAttack + enemy.attackCooldown)
            return true;
        return false;
    }

}
