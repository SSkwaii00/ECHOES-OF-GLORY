using UnityEngine;

public class Astra : Combatant
{
    protected override void Awake()
    {
        base.Awake();
        CombatantData data = ScriptableObject.CreateInstance<CombatantData>();
        data.Name = "Astra";
        data.HP = 90;
        data.Attack = 14;
        data.Agility = 70;
        data.Element = "Ice";
        data.Path = "Nihility";
        data.ArtPoints = 90;
        SetData(data);
    }

    public override int CalculateDamage(int actionIndex, Enemy target)
    {
        int damage = actionIndex switch
        {
            0 => Attack, // Attack
            1 => (int)Mathf.RoundToInt(Attack * 2.3f), // Skill: 2.3x
            2 => (int)Mathf.RoundToInt(Attack * 1.7f), // Util: 1.7x
            _ => Attack
        };
        return damage;
    }
}