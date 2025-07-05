using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatantManager : MonoBehaviour
{
    public List<Combatant> PlayerCombatants { get; private set; } = new List<Combatant>();
    public List<Enemy> EnemyCombatants { get; private set; } = new List<Enemy>();

    private TeamData teamData;
    private List<Transform> playerSpawnPoints;
    private List<Transform> enemySpawnPoints;

    public void SetTeamData(TeamData data)
    {
        teamData = data;
    }

    public void SetSpawnPoints(List<Transform> playerPoints, List<Transform> enemyPoints)
    {
        playerSpawnPoints = playerPoints;
        enemySpawnPoints = enemyPoints;
    }

    public void SetupCombatants()
    {
        if (teamData == null || playerSpawnPoints == null || enemySpawnPoints == null)
        {
            DebugLogger.LogError("CombatantManager: TeamData or SpawnPoints not set.");
            return;
        }

        PlayerCombatants.Clear();
        EnemyCombatants.Clear();

        // Shuffle spawn points to randomize placement
        List<Transform> availablePlayerSpawns = new List<Transform>(playerSpawnPoints);
        List<Transform> availableEnemySpawns = new List<Transform>(enemySpawnPoints);

        // Spawn Player Combatants (up to 4)
        int playerCount = Mathf.Min(teamData.SelectedCombatants.Count, 4);
        for (int i = 0; i < playerCount; i++)
        {
            if (i >= availablePlayerSpawns.Count) break;
            var combatantData = teamData.SelectedCombatants[i];
            if (combatantData != null && combatantData.Prefab != null)
            {
                GameObject playerObj = Instantiate(combatantData.Prefab, availablePlayerSpawns[i].position, Quaternion.identity);
                Combatant combatant = playerObj.GetComponent<Combatant>();
                if (combatant != null)
                {
                    combatant.SetData(combatantData);
                    PlayerCombatants.Add(combatant);

                    // Add CharacterUI
                    GameObject uiPrefab = Resources.Load<GameObject>("Prefabs/CharacterUI");
                    if (uiPrefab != null)
                    {
                        GameObject uiObj = Instantiate(uiPrefab, combatant.transform.position + Vector3.up * 2f, Quaternion.identity, combatant.transform);
                        CharacterUIManager uiManager = uiObj.GetComponent<CharacterUIManager>();
                        if (uiManager != null) uiManager.SetCombatant(combatant);
                    }
                }
            }
        }

        // Spawn Enemy Combatants (1-5, tùy số lượng trong TeamData)
        int enemyCount = Mathf.Min(teamData.SelectedEnemies.Count, enemySpawnPoints.Count);
        for (int i = 0; i < enemyCount; i++)
        {
            var enemyData = teamData.SelectedEnemies[i];
            if (enemyData != null && enemyData.Prefab != null)
            {
                GameObject enemyObj = Instantiate(enemyData.Prefab, availableEnemySpawns[i].position, Quaternion.identity);
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.SetData(enemyData);
                    EnemyCombatants.Add(enemy);

                    // Add CharacterUI
                    GameObject uiPrefab = Resources.Load<GameObject>("Prefabs/CharacterUI");
                    if (uiPrefab != null)
                    {
                        GameObject uiObj = Instantiate(uiPrefab, enemy.transform.position + Vector3.up * 2f, Quaternion.identity, enemy.transform);
                        CharacterUIManager uiManager = uiObj.GetComponent<CharacterUIManager>();
                        if (uiManager != null) uiManager.SetCombatant(enemy);
                    }
                }
            }
        }
    }

    public bool IsBattleOver()
    {
        return PlayerCombatants.All(p => p == null || p.HP <= 0) || EnemyCombatants.All(e => e == null || e.HP <= 0);
    }
}