using UnityEngine;


[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField]private float healPrecent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats=PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healamount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPrecent);

        playerStats.IncreaseHealtyBy(healamount);
    }
}
