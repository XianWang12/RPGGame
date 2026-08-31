using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BossTrigger : Enemy_SkeletonAnimationTriggers
{
    private Enemy_Boss boss=> GetComponentInParent<Enemy_Boss>();

    protected override void AnimationTrigger()
    {
        boss.AnimationFinishTrigger();
    }

    protected override void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(boss.attackCheck.position, boss.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats _target = hit.GetComponent<PlayerStats>();

                boss.stats.DoDamage(_target);
            }
    }

    private void Relocate()=>boss.FindPosition();

    private void MakeInvisible()=>boss.fx.MakeTransprent(true);

    private void MakeVisible()=>boss.fx.MakeTransprent(false);
}
