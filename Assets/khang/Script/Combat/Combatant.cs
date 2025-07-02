using UnityEngine;

public class Combatant : MonoBehaviour, ICombatant
{
    [SerializeField] private CombatantData data;
    private int hp;
    private int mana;
    private int energy;
    private int skillCharge;
    private float actionValue = 1f;
    private float slowAmount;
    private int slowTurnsRemaining;

    public string Name => data?.Name ?? "Unknown";
    public int HP
    {
        get => hp;
        set => hp = Mathf.Clamp(value, 0, data?.HP ?? 1000);
    }
    public int Mana
    {
        get => mana;
        set => mana = Mathf.Clamp(value, 0, 1000);
    }
    public int Energy
    {
        get => energy;
        set => energy = Mathf.Clamp(value, 0, 100);
    }
    public int SkillCharge // Thêm thuộc tính công khai cho SkillCharge
    {
        get => skillCharge;
        private set => skillCharge = value;
    }
    public float ActionValue => actionValue; // Triển khai ActionValue
    public AttackType AttackType => data?.AttackType ?? AttackType.Melee; // Thêm AttackType từ data
    public void SetData(CombatantData d) { data = d; HP = data.HP; Mana = 0; Energy = 0; SkillCharge = 0; }
    public CombatantData GetData() => data;

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public void ApplySlow(float amount, int duration)
    {
        slowAmount = amount;
        slowTurnsRemaining = duration;
    }

    public void UpdateStatus()
    {
        if (slowTurnsRemaining > 0)
        {
            actionValue = 1f / slowAmount;
            slowTurnsRemaining--;
        }
        else
        {
            actionValue = 1f;
        }
    }

    public virtual (int damage, string skillName, bool isAoE, float slowChance) CalculateDamage(int actionIndex, ICombatant target)
    {
        if (data == null || actionIndex < 0 || actionIndex >= data.Skills.Length) return (0, "N/A", false, 0f);
        SkillData skill = data.Skills[actionIndex];
        int baseDamage = Mathf.RoundToInt(data.Attack * skill.DamageMultiplier * actionValue);
        float critMultiplier = Random.value < data.CritRate ? 1.5f : 1f;
        int damage = Mathf.RoundToInt(baseDamage * critMultiplier);
        return (damage, skill.SkillName, skill.IsAoE, skill.StatusEffect == StatusEffect.Slow ? 0.3f : 0f);
    }

    public void GainSkillCharge(int amount) { SkillCharge = Mathf.Min(SkillCharge + amount, 3); }
    public void ResetSkillCharge() { SkillCharge = 0; }
    public void GainMana(int amount) { Mana = mana + amount; }
    public void ResetMana() { Mana = 0; }
    public void GainEnergy(int amount) { Energy = energy + amount; }
    public void ResetActionValue() { actionValue = 1f; }
    public void AdvanceActionValue() // Triển khai AdvanceActionValue
    {
        actionValue += 0.1f; // Tăng actionValue theo logic turn-based
    }
}