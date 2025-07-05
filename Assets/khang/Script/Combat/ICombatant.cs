using UnityEngine;

public interface ICombatant
{
    string Name { get; }
    int HP { get; set; }
    int Mana { get; set; }
    int Energy { get; set; }
    float ActionValue { get; } // Thêm thuộc tính ActionValue
    void TakeDamage(int damage);
    void ApplySlow(float amount, int duration);
    void UpdateStatus();
    void AdvanceActionValue(); // Thêm phương thức AdvanceActionValue
    (int damage, string skillName, bool isAoE, float slowChance) CalculateDamage(int actionIndex, ICombatant target);
}