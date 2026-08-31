using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    public float itemCooldown;
    public ItemEffect[] itemEffects;


    [Header("Major stats")]
    public int vitality;//生命力//3-5点生命值
    public int strength;//力量//1点伤害和1%爆伤
    public int agility;//敏捷//1%点闪避和1%暴击率
    public int intelligence;//智力//1点法伤3法防

    [Header("Offensive stats")]
    public int damage;
    public int critChance;//暴击率//
    public int critPower;//暴击伤害//

    [Header("Defensive stats")]
    public int maxHealth;////
    public int armor;//盔甲//
    public int evasion;//闪避//
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        int oldMaxHealth = playerStats.GetMaxHealthValue();

        playerStats.vitality.AddModifier(vitality);
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);

        int newMaxHealth = playerStats.GetMaxHealthValue();
        int maxHealthDelta = newMaxHealth - oldMaxHealth;

        playerStats.currentHealth += maxHealthDelta;
        if (playerStats.currentHealth > newMaxHealth)
            playerStats.currentHealth = newMaxHealth;
        if (playerStats.currentHealth < 0)
            playerStats.currentHealth = 0;

        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public void RemoveModifiers() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        int oldMaxHealth = playerStats.GetMaxHealthValue();

        playerStats.vitality.RemoveModifier(vitality);
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);

        int newMaxHealth = playerStats.GetMaxHealthValue();
        int maxHealthDelta = newMaxHealth - oldMaxHealth;

        playerStats.currentHealth += maxHealthDelta;
        if (playerStats.currentHealth > newMaxHealth)
            playerStats.currentHealth = newMaxHealth;
        if (playerStats.currentHealth < 0)
            playerStats.currentHealth = 0;

        if (playerStats.onHealthChanged != null)
            playerStats.onHealthChanged();
    }

    public override string GetDestription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "Strength");
        AddItemDescription(vitality, "vitality");
        AddItemDescription(agility, "agility");
        AddItemDescription(intelligence, "intellgence");
        AddItemDescription(strength, "Srength");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Crit.Chance");
        AddItemDescription(critPower, "Crit.Power");

        AddItemDescription(maxHealth, "maxHealth");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(armor, "Armor");
        AddItemDescription(magicResistance, "Magic Resist.");

        AddItemDescription(fireDamage, "Fire damage");
        AddItemDescription(iceDamage, "Ice damage");
        AddItemDescription(lightingDamage, "Lighting dmg. ");

        AddEffectDescription();

        if(descriptionLength<5)
        {
            for (int i = 0 ; i < 5-descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value ,string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            
            if(_value>0)
                sb.Append("+ "+ _value +" "+ _name);
            descriptionLength++;
        }
    }

    private void AddEffectDescription()
    {
        if (itemEffects == null || itemEffects.Length == 0)
            return;

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i] == null)
                continue;

            if (sb.Length > 0)
                sb.AppendLine();

            sb.Append("Effect: ");
            sb.Append(itemEffects[i].name.Replace('_', ' '));
            descriptionLength++;
        }
    }
}
