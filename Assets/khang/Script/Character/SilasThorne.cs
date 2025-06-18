using UnityEngine;

public class SilasThorne : Combatant
{
    protected override void Awake()
    {
        base.Awake();
        CombatantData data = ScriptableObject.CreateInstance<CombatantData>();
        data.Name = "Silas Thorne";
        data.HP = 110;
        data.Attack = 12;
        data.Agility = 50;
        data.Element = "Fire";
        data.Path = "Destruction";
        data.ArtPoints = 70;
        SetData(data);
    }

    public override int CalculateDamage(int actionIndex, Enemy target)
    {
        int damage = actionIndex switch
        {
            0 => Attack, // Attack
            1 => (int)Mathf.RoundToInt(Attack * 2.2f), // Skill: 2.2x
            2 => (int)Mathf.RoundToInt(Attack * 1.6f), // Util: 1.6x
            _ => Attack
        };
        return damage;
    }
}