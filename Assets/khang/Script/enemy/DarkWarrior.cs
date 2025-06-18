using UnityEngine;

public class DarkWarrior : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.Name = "DarkWarrior";
        data.HP = 150;
        data.Attack = 16;
        data.Defense = 10;
        data.Agility = 25;
        data.Element = "Dark";
        data.Path = "Preservation";
        data.ArtPoints = 40;
        SetData(data);
    }

    public override int CalculateDamage(Combatant target)
    {
        return (int)Mathf.RoundToInt(Attack * 1.3f);
    }
}