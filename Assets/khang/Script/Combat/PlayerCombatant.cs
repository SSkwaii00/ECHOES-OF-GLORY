using UnityEngine;

public class PlayerCombatant : Combatant
{
    // Xóa override khỏi Awake
    private void Awake()
    {
        // Thêm logic khởi tạo riêng nếu cần
    }

    public override (int damage, string skillName, bool isAoE, float slowChance) CalculateDamage(int actionIndex, ICombatant target)
    {
        var (damage, skillName, isAoE, slowChance) = base.CalculateDamage(actionIndex, target);
        // Thêm logic đặc biệt cho PlayerCombatant nếu cần (ví dụ: tăng sát thương dựa trên Energy)
        if (Energy > 50) damage += Mathf.RoundToInt(damage * 0.1f); // Tăng 10% sát thương nếu Energy > 50
        return (damage, skillName, isAoE, slowChance);
    }
}