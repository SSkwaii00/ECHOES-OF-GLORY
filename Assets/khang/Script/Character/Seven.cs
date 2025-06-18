using UnityEngine;

public class Seven : Combatant
{
    protected override void Awake()
    {
        base.Awake();
        CombatantData data = ScriptableObject.CreateInstance<CombatantData>();
        data.Name = "Seven";
        data.HP = 100;
        data.Attack = 15;
        data.Agility = 60;
        data.Element = "Lightning";
        data.Path = "Erudition";
        data.ArtPoints = 80;
        SetData(data);
    }

    public override int CalculateDamage(int actionIndex, Enemy target)
    {
        int damage = actionIndex switch
        {
            0 => Attack, // Attack
            1 => (int)Mathf.RoundToInt(Attack * 2.5f), // Skill: 2.5x
            2 => (int)Mathf.RoundToInt(Attack * 1.8f), // Util: 1.8x
            _ => Attack
        };
        return damage;
    }
}