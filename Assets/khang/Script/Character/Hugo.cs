using UnityEngine;

public class Hugo : Combatant
{
    protected override void Awake()
    {
        base.Awake();
        CombatantData data = ScriptableObject.CreateInstance<CombatantData>();
        data.Name = "Hugo";
        data.HP = 120;
        data.Attack = 10;
        data.Agility = 55;
        data.Element = "Physical";
        data.Path = "Preservation";
        data.ArtPoints = 60;
        SetData(data);
    }

    public override int CalculateDamage(int actionIndex, Enemy target)
    {
        int damage = actionIndex switch
        {
            0 => Attack, // Attack
            1 => (int)Mathf.RoundToInt(Attack * 2.0f), // Skill: 2.0x
            2 => (int)Mathf.RoundToInt(Attack * 1.5f), // Util: 1.5x
            _ => Attack
        };
        return damage;
    }
}