using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform Player;
    private Enemy_Skeleton enemy;
    private int moveDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        Player=PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        float distanceToPlayer = Vector2.Distance(Player.position, enemy.transform.position);

        if (enemy.IsPlayerDetected())
            stateTimer = enemy.battleTime;
        else
        {
            if (stateTimer < 0 || Vector2.Distance(Player.transform.position, enemy.transform.position) > 15)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (distanceToPlayer < enemy.attackDistance && CanAttack())
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        float xDistance = Mathf.Abs(Player.position.x - enemy.transform.position.x);
        float yDistance = Mathf.Abs(Player.position.y - enemy.transform.position.y);
        float overlapDeadZone = .2f;
        float directionChangeDeadZone = .35f;

        if (yDistance > 1f && xDistance <= 1f)
        {
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);
            return;
        }

        if (xDistance <= overlapDeadZone && yDistance <= 1f)
        {
            enemy.SetZeroVelocity();
            return;
        }

        if (Player.position.x > enemy.transform.position.x + directionChangeDeadZone)
            moveDir = 1;
        else if (Player.position.x < enemy.transform.position.x - directionChangeDeadZone)
            moveDir = -1;
        else
            moveDir = enemy.facingDir;

        enemy.SetVelocity(moveDir * enemy.moveSpeed * 1.2f, rb.velocity.y);
    }
    public bool CanAttack()
    {
        if (Time.time > enemy.lastTimeAttack + enemy.attackCooldown)
            return true;
        return false;
    }

}
