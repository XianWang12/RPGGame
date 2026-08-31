using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R)&&player.skill.blackhole.blackholeUnlocked&&player.skill.blackhole.cooldownTimer <= 0) 
            stateMachine.ChangeState(player.blackHole);

        if(Input.GetKeyDown(KeyCode.Mouse1)&&HasNoSword()&&player.skill.sword.swordUnlocked&&player.skill.sword.cooldownTimer <= 0)
            stateMachine.ChangeState(player.aimSword);

        if(Input.GetKeyDown(KeyCode.Q)&&player.skill.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttack);
        
        if(!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);
        
        if (Input.GetKeyDown(KeyCode.Space)&&player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if (player.sword==null)
            return true;

        AudioManager.instance.PlaySFX(17);
        AudioManager.instance.PlaySFX(27);
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false; 
    }
}
