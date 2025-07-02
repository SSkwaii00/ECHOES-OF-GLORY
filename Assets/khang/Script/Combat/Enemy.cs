using UnityEngine;

public class Enemy : MonoBehaviour, ICombatant
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Animator animator;

    private int hp;
    private int mana;
    private int energy;
    private float actionValue = 1f;
    private float slowAmount;
    private int slowTurnsRemaining;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                DebugLogger.LogError("Animator not found on " + gameObject.name);
            }
        }
    }

    void Start()
    {
        // Đảm bảo trạng thái ban đầu là Idle
        SetIdle();
    }

    public string Name => data?.Name ?? "Unknown";
    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, data?.HP ?? 1000);
            if (hp <= 0 && animator != null)
            {
                animator.SetBool("IsDead", true);
                DebugLogger.Log($"{Name} triggered Dead animation.");
            }
        }
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
    public float ActionValue => actionValue;
    public AttackType AttackType => data?.AttackType ?? AttackType.Melee;

    public void SetData(EnemyData d)
    {
        data = d;
        HP = data.HP;
        Mana = 0;
        Energy = 0;
        SetIdle(); // Đặt trạng thái Idle khi khởi tạo
    }

    public EnemyData GetData() => data;

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (damage > 0 && animator != null && HP > 0)
        {
            animator.SetTrigger("Hit");
            DebugLogger.Log($"{Name} triggered Hit animation.");
        }
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
        // Đảm bảo trạng thái Idle nếu không di chuyển
        if (animator != null && !animator.GetBool("IsMoving") && !animator.GetBool("IsDead"))
        {
            SetIdle();
        }
    }

    public (int damage, string skillName, bool isAoE, float slowChance) CalculateDamage(int actionIndex, ICombatant target)
    {
        if (data == null || actionIndex < 0 || actionIndex >= data.Skills.Length) return (0, "N/A", false, 0f);
        SkillData skill = data.Skills[actionIndex];
        int baseDamage = Mathf.RoundToInt(data.Attack * skill.DamageMultiplier * actionValue);
        float critMultiplier = Random.value < 0.05f ? 1.5f : 1f;
        int damage = Mathf.RoundToInt(baseDamage * critMultiplier);
        return (damage, skill.SkillName, skill.IsAoE, skill.StatusEffect == StatusEffect.Slow ? 0.3f : 0f);
    }

    public void GainMana(int amount) { Mana = mana + amount; }
    public void GainEnergy(int amount) { Energy = energy + amount; }
    public void ResetMana() { Mana = 0; }
    public void ResetActionValue() { actionValue = 1f; }
    public void AdvanceActionValue()
    {
        actionValue += 0.1f;
    }

    public void SetMoving(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            DebugLogger.Log($"{Name} set IsMoving to {isMoving}");
            if (!isMoving && HP > 0)
            {
                SetIdle(); // Chuyển về Idle nếu không di chuyển và còn sống
            }
        }
    }

    public void SetAttacking(int actionIndex)
    {
        if (animator != null)
        {
            string trigger = actionIndex switch
            {
                0 => "Attack1",
                1 => "Attack2",
                2 => "Attack3",
                _ => "Attack1"
            };
            animator.SetTrigger(trigger);
            animator.SetBool("IsMoving", false); // Tắt Run khi tấn công
            DebugLogger.Log($"{Name} triggered {trigger} animation.");
        }
    }

    public void ResetAttacking()
    {
        if (animator != null)
        {
            DebugLogger.Log($"{Name} reset attacking state.");
            if (HP > 0)
            {
                SetIdle(); // Quay lại Idle sau khi tấn công
            }
        }
    }

    private void SetIdle()
    {
        if (animator != null && !animator.GetBool("IsDead"))
        {
            animator.SetBool("IsMoving", false);
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
            DebugLogger.Log($"{Name} set to Idle.");
        }
    }
}