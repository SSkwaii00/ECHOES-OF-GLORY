using UnityEngine;

public enum AttackRange { Melee, Ranged }
public enum StatusEffect { None, Freeze, Burn, Slow, Panic, Heal, Shield }

[CreateAssetMenu(fileName = "CombatantData", menuName = "Data/CombatantData")]
public class CombatantData : ScriptableObject
{
    public string Name;
    public int HP;
    public int Attack;
    public int Agility;
    public float CritRate;
    public string Element;
    public string Path;
    public AttackType AttackType;
    public AttackRange AttackRange;
    public float SlowAmount;
    public int SlowDuration;
    public SkillData[] Skills = new SkillData[3];
    public int Skill3ManaCost = 100;
    public int Level = 1;
    public Sprite AvatarSprite;
    public GameObject Prefab;
}