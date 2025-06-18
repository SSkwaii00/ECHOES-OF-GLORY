using UnityEngine;

public class DarkMage : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.Name = "DarkMage";
        data.HP = 90;
        data.Attack = 14;
        data.Agility = 30;
        data.Element = "Dark";
        data.Path = "Abundance";
        data.ArtPoints = 70;
        SetData(data);
    }

    public override int CalculateDamage(Combatant target)
    {
        return (int)Mathf.RoundToInt(Attack * 1.2f);
    }
}