using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : EnemyState
{
    private Enemy_Boss enemy;
    private Player player;
    public BossIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player;
        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(player.transform.position, enemy.transform.position)<7)
        {
            enemy.bossFightStarted = true;
            stateMachine.ChangeState(enemy.battleState);
            return;
        }
        
        if(stateTimer<=0&&enemy.bossFightStarted)
            stateMachine.ChangeState(enemy.moveState);
    }
}
