using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public Transform[] characterPositions; // Mảng 4 vị trí cho character
    public Transform[] enemyPositions; // Mảng 5 vị trí cho enemy
    public EnemyData[] enemies; // Mảng để kéo thả prefab enemy
    public List<Combatant> playerCombatants; // Danh sách Combatant
    public List<Enemy> enemyCombatants; // Danh sách Enemy
    private List<(MonoBehaviour combatant, int agility, bool isPlayer)> turnOrder; // Thứ tự lượt
    private int currentTurnIndex = -1;
    private bool isPlayerTurn = false;

    // UI
    public Button attackButton;
    public Button skillButton;
    public Button utilButton;
    public GameObject actionPanel; // Panel chứa 3 nút

    // Vị trí tấn công
    public Transform attackPosition; // Vị trí di chuyển đến khi tấn công
    public float moveSpeed = 5f; // Tốc độ di chuyển

    void Start()
    {
        if (characterPositions == null || characterPositions.Length < 4)
        {
            Debug.LogError("Character positions not set or less than 4.");
            return;
        }
        if (enemyPositions == null || enemyPositions.Length < 1)
        {
            Debug.LogError("Enemy positions not set.");
            return;
        }
        if (attackButton == null || skillButton == null || utilButton == null || actionPanel == null)
        {
            Debug.LogError("UI buttons or action panel not assigned.");
            return;
        }
        if (attackPosition == null)
        {
            Debug.LogError("Attack position not assigned.");
            return;
        }

        playerCombatants = new List<Combatant>();
        enemyCombatants = new List<Enemy>();
        SetupBattle();
        InitializeTurnOrder();
        StartNextTurn();
    }

    void SetupBattle()
    {
        // Spawn character
        TeamSelection teamSelection = FindFirstObjectByType<TeamSelection>();
        if (teamSelection != null && TeamSelection.selectedCharacterIndices != null && teamSelection.characterPrefabs != null)
        {
            for (int i = 0; i < Mathf.Min(TeamSelection.selectedCharacterIndices.Count, 4); i++)
            {
                int characterIndex = TeamSelection.selectedCharacterIndices[i];
                if (characterPositions[i] != null && characterIndex >= 0 && characterIndex < teamSelection.characterPrefabs.Count)
                {
                    GameObject characterPrefab = teamSelection.characterPrefabs[characterIndex];
                    if (characterPrefab != null)
                    {
                        GameObject characterInstance = Instantiate(characterPrefab, characterPositions[i].position, Quaternion.identity);
                        characterInstance.transform.SetParent(characterPositions[i]);
                        Combatant combatant = characterInstance.GetComponent<Combatant>();
                        if (combatant != null)
                        {
                            combatant.SetData(teamSelection.availableCombatantData[characterIndex]);
                            playerCombatants.Add(combatant);
                        }
                    }
                }
            }
        }

        // Spawn enemy
        int enemyCount = Mathf.Min(enemies.Length, 5);
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyPositions[i] != null && enemies[i] != null && enemies[i].Prefab != null)
            {
                GameObject enemyInstance = Instantiate(enemies[i].Prefab, enemyPositions[i].position, Quaternion.identity);
                enemyInstance.transform.SetParent(enemyPositions[i]);
                Enemy enemy = enemyInstance.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.SetData(enemies[i]);
                    enemyCombatants.Add(enemy);
                }
            }
        }
    }

    void InitializeTurnOrder()
    {
        turnOrder = new List<(MonoBehaviour, int, bool)>();
        foreach (Combatant combatant in playerCombatants)
        {
            if (combatant != null) turnOrder.Add((combatant, combatant.Agility, true));
        }
        foreach (Enemy enemy in enemyCombatants)
        {
            if (enemy != null) turnOrder.Add((enemy, enemy.Agility, false));
        }
        turnOrder = turnOrder.OrderByDescending(t => t.agility).ThenBy(t => t.isPlayer ? 0 : 1).ToList(); // Ưu tiên player nếu Agility bằng nhau
    }

    void StartNextTurn()
    {
        // Kiểm tra chiến thắng/thua
        if (enemyCombatants.All(e => e == null || e.HP <= 0))
        {
            Debug.Log("Victory! All enemies defeated.");
            return;
        }
        if (playerCombatants.All(p => p == null || p.HP <= 0))
        {
            Debug.Log("Defeat! All players defeated.");
            return;
        }

        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        while (turnOrder[currentTurnIndex].combatant == null || (turnOrder[currentTurnIndex].isPlayer && playerCombatants.Find(p => p == turnOrder[currentTurnIndex].combatant) == null) || (!turnOrder[currentTurnIndex].isPlayer && enemyCombatants.Find(e => e == turnOrder[currentTurnIndex].combatant) == null))
        {
            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        }

        var current = turnOrder[currentTurnIndex];
        isPlayerTurn = current.isPlayer;
        if (isPlayerTurn)
        {
            Combatant combatant = current.combatant as Combatant;
            actionPanel.SetActive(true);
            attackButton.onClick.RemoveAllListeners();
            skillButton.onClick.RemoveAllListeners();
            utilButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(() => StartCoroutine(PerformPlayerAction(combatant, 0)));
            skillButton.onClick.AddListener(() => StartCoroutine(PerformPlayerAction(combatant, 1)));
            utilButton.onClick.AddListener(() => StartCoroutine(PerformPlayerAction(combatant, 2)));
            UpdateButtonStates(combatant);
        }
        else
        {
            Enemy enemy = current.combatant as Enemy;
            actionPanel.SetActive(false);
            StartCoroutine(PerformEnemyAction(enemy));
        }
    }

    void UpdateButtonStates(Combatant combatant)
    {
        skillButton.interactable = combatant.SkillCharge >= 3;
        utilButton.interactable = combatant.Mana >= 100;
    }

    IEnumerator PerformPlayerAction(Combatant combatant, int actionIndex)
    {
        actionPanel.SetActive(false);
        Enemy target = enemyCombatants.FirstOrDefault(e => e != null && e.HP > 0);
        if (target == null) yield break;

        Vector3 originalPosition = combatant.transform.position;
        yield return StartCoroutine(MoveToPosition(combatant.transform, attackPosition.position));

        int damage = 0;
        if (actionIndex == 0) // Attack
        {
            damage = combatant.CalculateDamage(0, target);
            combatant.GainSkillCharge(1);
            combatant.GainMana(20);
        }
        else if (actionIndex == 1 && combatant.SkillCharge >= 3) // Skill
        {
            damage = combatant.CalculateDamage(1, target);
            combatant.ResetSkillCharge();
        }
        else if (actionIndex == 2 && combatant.Mana >= 100) // Util
        {
            damage = combatant.CalculateDamage(2, target);
            combatant.ResetMana();
        }

        target.TakeDamage(damage);
        Debug.Log($"{combatant.Name} (HP: {combatant.HP}, MP: {combatant.Mana}) deals {damage} damage to {target.Name}");

        if (target.HP <= 0)
        {
            Destroy(target.gameObject);
            enemyCombatants.Remove(target);
        }

        yield return StartCoroutine(MoveToPosition(combatant.transform, originalPosition));
        StartNextTurn();
    }

    IEnumerator PerformEnemyAction(Enemy enemy)
    {
        Combatant target = playerCombatants.FirstOrDefault(p => p != null && p.HP > 0);
        if (target == null) yield break;

        Vector3 originalPosition = enemy.transform.position;
        yield return StartCoroutine(MoveToPosition(enemy.transform, attackPosition.position));

        int damage = enemy.CalculateDamage(target);
        target.TakeDamage(damage);
        target.GainMana(20);
        Debug.Log($"{enemy.Name} (HP: {enemy.HP}, MP: 0) deals {damage} damage to {target.Name}");

        if (target.HP <= 0)
        {
            Destroy(target.gameObject);
            playerCombatants.Remove(target);
        }

        yield return StartCoroutine(MoveToPosition(enemy.transform, originalPosition));
        StartNextTurn();
    }

    IEnumerator MoveToPosition(Transform transform, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.position = targetPosition;
    }
}