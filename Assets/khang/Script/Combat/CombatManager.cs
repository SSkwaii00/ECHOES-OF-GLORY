using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private TeamData teamData;
    [SerializeField] private List<Transform> playerSpawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    [SerializeField] private UIManager uiManager;

    private CombatantManager combatantManager;
    private TurnManager turnManager;
    private MovementManager movementManager;
    private List<(string characterName, string skillName, int damage)> battleHistory = new List<(string, string, int)>();
    private int turnCount;
    private bool isPlayerTurnFirst;

    void Awake()
    {
        combatantManager = gameObject.AddComponent<CombatantManager>();
        turnManager = gameObject.AddComponent<TurnManager>();
        movementManager = gameObject.AddComponent<MovementManager>();
        if (uiManager == null)
        {
            DebugLogger.LogError("UIManager not assigned. Please assign UIManagerObject in Inspector.");
        }
        else
        {
            uiManager.SetCombatManager(this);
        }

        combatantManager.SetTeamData(teamData);
        combatantManager.SetSpawnPoints(playerSpawnPoints, enemySpawnPoints);
        movementManager.SetCombatManager(this);

        isPlayerTurnFirst = PlayerPrefs.GetInt("PlayerTurnFirst", 1) == 1;
        DebugLogger.Log($"isPlayerTurnFirst: {isPlayerTurnFirst}");
    }

    void Start()
    {
        Debug.Log("Combatants initialized. Player count: " + combatantManager.PlayerCombatants.Count + ", Enemy count: " + combatantManager.EnemyCombatants.Count);
        ValidateSetup();
        combatantManager.SetupCombatants();
        Debug.Log("After setup - Player count: " + combatantManager.PlayerCombatants.Count + ", Enemy count: " + combatantManager.EnemyCombatants.Count);

        foreach (var player in combatantManager.PlayerCombatants.Where(p => p != null && p.HP > 0))
        {
            var nearestEnemy = combatantManager.EnemyCombatants.Where(e => e != null && e.HP > 0)
                .OrderBy(e => Vector3.Distance(player.transform.position, e.transform.position))
                .FirstOrDefault();
            if (nearestEnemy != null)
            {
                player.transform.LookAt(nearestEnemy.transform.position);
                Debug.Log(player.Name + " rotated to face " + nearestEnemy.Name);
            }
        }

        foreach (var enemy in combatantManager.EnemyCombatants.Where(e => e != null && e.HP > 0))
        {
            var nearestPlayer = combatantManager.PlayerCombatants.Where(p => p != null && p.HP > 0)
                .OrderBy(p => Vector3.Distance(enemy.transform.position, p.transform.position))
                .FirstOrDefault();
            if (nearestPlayer != null)
            {
                enemy.transform.LookAt(nearestPlayer.transform.position);
                Debug.Log(enemy.Name + " rotated to face " + nearestPlayer.Name);
            }
        }

        turnManager.Initialize(combatantManager.PlayerCombatants, combatantManager.EnemyCombatants, isPlayerTurnFirst);
        if (!isPlayerTurnFirst)
        {
            StartCoroutine(PerformEnemyFirstTurn());
        }
        else
        {
            StartNextTurn();
        }
    }

    private void ValidateSetup()
    {
        if (teamData == null || teamData.SelectedCombatants.Count == 0 || teamData.SelectedEnemies.Count == 0)
        {
            DebugLogger.LogError("Invalid TeamData or no combatants/enemies selected.");
        }
        if (playerSpawnPoints.Count < 4)
        {
            DebugLogger.LogError("Need at least 4 player spawn points.");
        }
        if (enemySpawnPoints.Count < 1)
        {
            DebugLogger.LogError("Need at least 1 enemy spawn point.");
        }
    }

    public void StartNextTurn()
    {
        if (combatantManager.IsBattleOver())
        {
            ShowBattleResult();
            return;
        }

        var turnInfo = turnManager.GetNextTurn(combatantManager.PlayerCombatants, combatantManager.EnemyCombatants);
        if (turnInfo == null)
        {
            DebugLogger.LogError("No valid turn info returned.");
            return;
        }

        var (combatant, isPlayer) = turnInfo.Value;
        turnCount++;
        uiManager.UpdateTurnCount(turnCount);
        DebugLogger.Log($"Starting turn {turnCount} for {combatant.Name} (Player: {isPlayer})");

        if (isPlayer)
        {
            if (combatant is Combatant player && player.HP > 0)
            {
                uiManager.ShowActionPanel(player, (actionIndex) => StartCoroutine(PerformPlayerAction(player, actionIndex)));
            }
            else
            {
                StartNextTurn();
            }
        }
        else
        {
            if (combatant is Enemy enemy && enemy.HP > 0)
            {
                uiManager.HideActionPanel();
                StartCoroutine(PerformEnemyAction(enemy, true));
            }
            else
            {
                StartNextTurn();
            }
        }
    }

    private IEnumerator PerformEnemyFirstTurn()
    {
        var liveEnemies = combatantManager.EnemyCombatants.Where(e => e != null && e.HP > 0).ToList();
        if (liveEnemies.Count == 0)
        {
            DebugLogger.Log("No live enemies to perform first turn.");
            StartNextTurn();
            yield break;
        }

        var enemy = liveEnemies[0];
        uiManager.HideActionPanel();
        DebugLogger.Log($"Enemy first turn: {enemy.Name}");
        yield return StartCoroutine(PerformEnemyAction(enemy, false));
        turnManager.ForcePlayerTurn();
        StartNextTurn();
    }

    public void OnEnemyClicked(Enemy enemy)
    {
        if (combatantManager.PlayerCombatants.Any(p => p != null && p.HP > 0))
        {
            turnManager.TriggerPlayerTurn(combatantManager.PlayerCombatants, combatantManager.EnemyCombatants);
            var turnInfo = turnManager.GetNextTurn(combatantManager.PlayerCombatants, combatantManager.EnemyCombatants);
            if (turnInfo.HasValue && turnInfo.Value.Item2)
            {
                var (combatant, _) = turnInfo.Value;
                if (combatant is Combatant player && player.HP > 0)
                {
                    StartCoroutine(PerformPlayerAction(player, 0));
                }
            }
        }
    }

    private IEnumerator PerformPlayerAction(Combatant combatant, int actionIndex)
    {
        uiManager.HideActionPanel();
        Enemy target = combatantManager.EnemyCombatants.FirstOrDefault(e => e != null && e.HP > 0);
        if (target == null) yield break;

        CombatantData data = combatant.GetData();
        Vector3 originalPosition = combatant.transform.position;
        float moveDistance = data.AttackRange == AttackRange.Melee ? 0f : 0f;

        if (data.AttackRange == AttackRange.Melee)
        {
            yield return StartCoroutine(movementManager.MoveForAction(combatant.transform, target.transform, combatant.AttackType, moveDistance));
        }

        var (damage, skillName, isAoE, slowChance) = combatant.CalculateDamage(actionIndex, target);
        float comboMultiplier = CheckCombo(combatant.Name, skillName);
        damage = Mathf.RoundToInt(damage * comboMultiplier);
        bool isCritical = data != null && Random.value < data.CritRate;

        SkillData skill = data?.Skills[actionIndex];
        if (skill != null && Random.value < skill.StatusEffectChance)
        {
            ApplyStatusEffect(target, skill.StatusEffect, skill.StatusEffectDuration, combatant);
        }

        if (actionIndex == 0)
        {
            combatant.GainSkillCharge(1);
            combatant.GainMana(20);
            combatant.GainEnergy(10);
        }
        else if (actionIndex == 1 && combatant.SkillCharge >= 3)
        {
            combatant.ResetSkillCharge();
        }
        else if (actionIndex == 2 && combatant.Mana >= data.Skill3ManaCost)
        {
            combatant.ResetMana();
            if (combatant.Name == "Rex" && !isAoE && target.HP <= damage) combatant.ResetActionValue();
        }

        if (isAoE)
        {
            var liveEnemies = combatantManager.EnemyCombatants.Where(e => e != null && e.HP > 0).ToList();
            foreach (var enemy in liveEnemies)
            {
                ApplyDamageAndEffects(combatant, enemy, damage, skillName, isCritical, slowChance);
            }
        }
        else
        {
            ApplyDamageAndEffects(combatant, target, damage, skillName, isCritical, slowChance);
        }

        if (data.AttackRange == AttackRange.Melee)
        {
            yield return StartCoroutine(movementManager.MoveForAction(combatant.transform, combatant.transform, combatant.AttackType, 0f, true, originalPosition));
        }

        combatant.ResetActionValue();
        yield return new WaitForSeconds(1f);
        StartNextTurn();
    }

    private IEnumerator PerformEnemyAction(Enemy enemy, bool callNextTurn)
    {
        enemy.UpdateStatus();
        Combatant target = combatantManager.PlayerCombatants.FirstOrDefault(p => p != null && p.HP > 0);
        if (target == null) yield break;

        EnemyData data = enemy.GetData();
        Vector3 originalPosition = enemy.transform.position;
        float moveDistance = data.AttackRange == AttackRange.Melee ? 0f : 0f;

        // Kích hoạt animation Run và di chuyển đến mục tiêu
        if (data.AttackRange == AttackRange.Melee)
        {
            enemy.SetMoving(true);
            yield return StartCoroutine(movementManager.MoveForAction(enemy.transform, target.transform, enemy.AttackType, moveDistance));
            enemy.SetMoving(false); // Tắt Run khi đến vị trí
        }

        // Xác định hành động và kích hoạt animation Attack
        int actionIndex = DetermineEnemyAction(enemy);
        UpdateEnemyResources(enemy, actionIndex); // Cập nhật Mana/Energy
        enemy.SetAttacking(actionIndex);

        var (damage, skillName, isAoE, slowChance) = enemy.CalculateDamage(actionIndex, target);
        bool isCritical = Random.value < 0.05f;

        SkillData skill = data?.Skills?[actionIndex];
        if (skill != null && Random.value < skill.StatusEffectChance)
        {
            ApplyStatusEffect(target, skill.StatusEffect, skill.StatusEffectDuration, enemy);
        }

        // Chờ animation tấn công hoàn tất
        float animationDuration = skill != null && skill.AnimationTrigger != null ? GetAnimationDuration(enemy, skill.AnimationTrigger) : 1f;
        yield return new WaitForSeconds(animationDuration);

        // Áp dụng sát thương sau khi animation tấn công hoàn tất
        if (isAoE)
        {
            var livePlayers = combatantManager.PlayerCombatants.Where(p => p != null && p.HP > 0).ToList();
            DebugLogger.Log($"Applying AoE to {livePlayers.Count} players");
            foreach (var player in livePlayers)
            {
                ApplyDamageAndEffects(enemy, player, damage, skillName, isCritical, slowChance);
            }
        }
        else
        {
            ApplyDamageAndEffects(enemy, target, damage, skillName, isCritical, slowChance);
        }

        enemy.ResetAttacking(); // Đặt lại trạng thái tấn công, chuyển về Idle

        // Di chuyển về vị trí ban đầu nếu là cận chiến
        if (data.AttackRange == AttackRange.Melee)
        {
            // Quay về hướng originalPosition trước khi di chuyển
            enemy.transform.LookAt(originalPosition);
            DebugLogger.Log($"{enemy.Name} rotated to face original position: {originalPosition}");

            enemy.SetMoving(true); // Kích hoạt Run khi quay về
            yield return StartCoroutine(movementManager.MoveForAction(enemy.transform, enemy.transform, enemy.AttackType, 0f, true, originalPosition));
            enemy.SetMoving(false); // Tắt Run và chuyển về Idle

            // Quay mặt lại về phía nhân vật gần nhất còn sống
            var nearestPlayer = combatantManager.PlayerCombatants
                .Where(p => p != null && p.HP > 0)
                .OrderBy(p => Vector3.Distance(enemy.transform.position, p.transform.position))
                .FirstOrDefault();
            if (nearestPlayer != null)
            {
                enemy.transform.LookAt(nearestPlayer.transform.position);
                DebugLogger.Log($"{enemy.Name} rotated to face {nearestPlayer.Name} after returning");
            }
        }

        enemy.ResetActionValue();
        yield return new WaitForSeconds(0.5f); // Chờ thêm để tạo nhịp
        if (callNextTurn)
        {
            StartNextTurn();
        }
    }

    private void ApplyDamageAndEffects(ICombatant attacker, ICombatant target, int damage, string skillName, bool isCritical, float slowChance)
    {
        target.TakeDamage(damage);
        DamagePopup.Create(((Component)target).transform.position + Vector3.up * 1f, damage, isCritical);
        battleHistory.Add((attacker.Name, skillName, damage));
        DebugLogger.Log($"{attacker.Name} (HP: {attacker.HP}, MP: {attacker.Mana}) uses {skillName} to deal {damage} to {target.Name}");

        if (slowChance > 0 && Random.value < slowChance)
        {
            float slowAmount = attacker is Combatant c ? c.GetData().SlowAmount : (attacker as Enemy).GetData().SlowAmount;
            int slowDuration = attacker is Combatant c2 ? c2.GetData().SlowDuration : (attacker as Enemy).GetData().SlowDuration;
            target.ApplySlow(slowAmount, slowDuration);
            DebugLogger.Log($"{target.Name} is slowed for {slowDuration} turns.");
        }

        if (target.HP <= 0)
        {
            if (target is Combatant player) combatantManager.PlayerCombatants.Remove(player);
            else if (target is Enemy enemy) combatantManager.EnemyCombatants.Remove(enemy);
            Destroy(((Component)target).gameObject);
        }
    }

    private void ApplyStatusEffect(ICombatant target, StatusEffect effect, int duration, ICombatant attacker)
    {
        switch (effect)
        {
            case StatusEffect.Freeze:
                target.ApplySlow(1f, duration);
                DebugLogger.Log($"{target.Name} is frozen for {duration} turns.");
                break;
            case StatusEffect.Burn:
                DebugLogger.Log($"{target.Name} is burning for {duration} turns.");
                break;
            case StatusEffect.Heal:
                int healAmount = Mathf.RoundToInt(target.HP * 0.1f);
                target.TakeDamage(-healAmount);
                DebugLogger.Log($"{target.Name} is healed for {healAmount} HP.");
                break;
            case StatusEffect.Panic:
                DebugLogger.Log($"{target.Name} is panicked for {duration} turns.");
                break;
            case StatusEffect.Shield:
                DebugLogger.Log($"{target.Name} gains a shield for {duration} turns.");
                break;
        }
    }

    private int DetermineEnemyAction(Enemy enemy)
    {
        EnemyData data = enemy.GetData();
        DebugLogger.Log($"{enemy.Name} Mana: {enemy.Mana}, Skill2ManaCost: {data.Skill2ManaCost}, Skill3ManaCost: {data.Skill3ManaCost}");
        if (enemy.Mana >= data.Skill3ManaCost && (Random.value < 0.5f || turnCount > 5))
        {
            DebugLogger.Log($"{enemy.Name} selected Skill3");
            return 2;
        }
        if (enemy.Mana >= data.Skill2ManaCost && (Random.value < 0.7f || turnCount > 3))
        {
            DebugLogger.Log($"{enemy.Name} selected Skill2");
            return 1;
        }
        DebugLogger.Log($"{enemy.Name} selected Attack1");
        return 0;
    }

    private void UpdateEnemyResources(Enemy enemy, int actionIndex)
    {
        EnemyData data = enemy.GetData();
        if (actionIndex == 0)
        {
            enemy.GainMana(30);
            enemy.GainEnergy(10);
        }
        else if (actionIndex == 1)
        {
            enemy.GainMana(-data.Skill2ManaCost);
        }
        else if (actionIndex == 2)
        {
            enemy.ResetMana();
        }
        DebugLogger.Log($"{enemy.Name} Mana updated to: {enemy.Mana}");
    }

    private float CheckCombo(string characterName, string skillName)
    {
        if (battleHistory.Count > 0 && battleHistory[battleHistory.Count - 1].characterName == "Astra" &&
            battleHistory[battleHistory.Count - 1].skillName == "Entropic Wave" &&
            characterName == "Hugo" && skillName == "Cataclysmic Blow")
        {
            DebugLogger.Log("Combo: Astra Entropic Wave -> Hugo Cataclysmic Blow (+20% damage)");
            return 1.2f;
        }
        return 1f;
    }

    private void ShowBattleResult()
    {
        ResultScreen resultScreen = FindFirstObjectByType<ResultScreen>();
        if (resultScreen != null)
        {
            bool victory = combatantManager.EnemyCombatants.All(e => e == null || e.HP <= 0);
            resultScreen.ShowResult(victory, battleHistory, turnCount);
        }
        else
        {
            SceneManager.LoadScene("Map");
        }
    }

    private float GetAnimationDuration(Enemy enemy, string animationTrigger)
    {
        if (enemy.GetComponent<Animator>() == null) return 1f;
        var controller = enemy.GetComponent<Animator>().runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name.Contains(animationTrigger))
                return clip.length;
        }
        return 1f;
    }
}