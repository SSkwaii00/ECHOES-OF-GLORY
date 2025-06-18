using UnityEngine;
using System.Collections.Generic;

public class Combatant : MonoBehaviour
{
    [SerializeField]
    private CombatantData data;

    public string Name { get => data?.Name ?? "Unknown"; set => data.Name = value; }
    public int Level { get => data?.Level ?? 1; set => data.Level = value; }
    public int HP { get => data?.HP ?? 100; set => data.HP = value; }
    public int Attack { get => data?.Attack ?? 10; set => data.Attack = value; }
    public int Defense { get => data?.Defense ?? 5; set => data.Defense = value; }
    public int Agility { get => data?.Agility ?? 50; set => data.Agility = value; }
    public float CritRate { get => data?.CritRate ?? 5.0f; set => data.CritRate = value; }
    public float CritDamage { get => data?.CritDamage ?? 50.0f; set => data.CritDamage = value; }
    public string Element { get => data?.Element ?? "None"; set => data.Element = value; }
    public string Path { get => data?.Path ?? "Default"; set => data.Path = value; }
    public int ArtPoints { get => data?.ArtPoints ?? 0; set => data.ArtPoints = value; }

    private int potentialEnergy = 0;
    public int PotentialEnergy { get => potentialEnergy; set => potentialEnergy = value; }
    private int skillCharge = 0;
    public int SkillCharge { get => skillCharge; set => skillCharge = value; }
    private int mana = 0;
    public int Mana { get => mana; set => mana = value; }

    private bool isStunned = false;
    private bool isSlowed = false;
    private bool isFrozen = false;
    private bool isPoisoned = false;
    private int shield = 0;

    public bool IsStunned { get => isStunned; set => isStunned = value; }
    public bool IsSlowed { get => isSlowed; set => isSlowed = value; }
    public bool IsFrozen { get => isFrozen; set => isFrozen = value; }
    public bool IsPoisoned { get => isPoisoned; set => isPoisoned = value; }
    public int Shield { get => shield; set => shield = value; }

    protected virtual void Awake() { }

    public void TakeDamage(int damage)
    {
        if (shield > 0)
        {
            shield -= damage;
            if (shield < 0)
            {
                damage = -shield;
                shield = 0;
            }
            else
            {
                damage = 0;
            }
        }
        HP -= damage;
        if (HP < 0) HP = 0;
    }

    public void SyncShield(int newShield)
    {
        shield = newShield;
    }

    public void GainEnergy(int amount)
    {
        PotentialEnergy += amount;
    }

    public void GainSkillCharge(int amount)
    {
        skillCharge += amount;
        if (skillCharge > 3) skillCharge = 3;
    }

    public void ResetSkillCharge()
    {
        skillCharge = 0;
    }

    public void GainMana(int amount)
    {
        mana += amount;
        if (mana > 100) mana = 100;
    }

    public void ResetMana()
    {
        mana = 0;
    }

    public virtual int CalculateDamage(int actionIndex, Enemy target)
    {
        int damage = actionIndex switch
        {
            0 => Attack, // Attack
            1 => (int)Mathf.RoundToInt(Attack * 2.0f), // Skill: Gấp đôi
            2 => (int)Mathf.RoundToInt(Attack * 1.5f), // Util: 1.5x
            _ => Attack
        };
        return damage;
    }

    public void SetData(CombatantData newData)
    {
        if (newData != null)
        {
            data = Instantiate(newData);
            Name = data.Name;
            Level = data.Level;
            HP = data.HP;
            Attack = data.Attack;
            Defense = data.Defense;
            Agility = data.Agility;
            CritRate = data.CritRate;
            CritDamage = data.CritDamage;
            Element = data.Element;
            Path = data.Path;
            ArtPoints = data.ArtPoints;
            PotentialEnergy = 0;
            skillCharge = 0;
            mana = 0;
        }
    }

    public CombatantData GetData()
    {
        return data;
    }
}