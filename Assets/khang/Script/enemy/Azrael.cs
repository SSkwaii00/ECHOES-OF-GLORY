using UnityEngine;

public class Azrael : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.Name = "Azrael";
        data.HP = 120;
        data.Attack = 12;
        data.Agility = 40;
        data.Element = "Dark";
        data.Path = "Destruction";
        data.ArtPoints = 50;
        SetData(data);
    }

    public override int CalculateDamage(Combatant target)
    {
        return (int)Mathf.RoundToInt(Attack * 1.5f);
    }
}