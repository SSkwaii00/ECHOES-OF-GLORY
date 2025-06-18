using UnityEngine;

public class RiftHorror : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.Name = "RiftHorror";
        data.HP = 80;
        data.Attack = 10;
        data.Agility = 80;
        data.Element = "Void";
        data.Path = "Nihility";
        data.ArtPoints = 60;
        SetData(data);
    }

    public override int CalculateDamage(Combatant target)
    {
        return Attack + (int)(Agility * 0.1f);
    }
}