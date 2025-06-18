using UnityEngine;

public class TheNightbringer : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.Name = "TheNightbringer";
        data.HP = 200;
        data.Attack = 20;
        data.Defense = 8;
        data.Agility = 35;
        data.Element = "Dark";
        data.Path = "Destruction";
        data.ArtPoints = 100;
        SetData(data);
    }

    public override int CalculateDamage(Combatant target)
    {
        return Attack * 2; // Gấp đôi sát thương
    }
}