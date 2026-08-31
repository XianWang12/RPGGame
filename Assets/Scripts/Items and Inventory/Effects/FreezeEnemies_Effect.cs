using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item effect/Freeze enemies effect")]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField] private float duration;
    
    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats playerstats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerstats.currentHealth>playerstats.GetMaxHealthValue()*.1f)
            return;

        if(!Inventory.Instance.CanUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);
        
        foreach(Collider2D hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
    }
}
