using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [SerializeField] private TeamData teamData;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject teamSetupPanel;
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private TMPro.TextMeshProUGUI enemyInfoText;
    [SerializeField] private float enemyAttackRange = 3f;

    private List<EnemyTrigger> enemies = new List<EnemyTrigger>();
    private bool isPlayerTurnFirst;

    void Start()
    {
        if (teamData == null || playerTransform == null || teamSetupPanel == null || enemyInfoPanel == null || enemyInfoText == null)
        {
            DebugLogger.LogError("MapManager components not assigned.");
            return;
        }

        teamSetupPanel.SetActive(false);
        enemyInfoPanel.SetActive(false);
        enemies.AddRange(FindObjectsByType<EnemyTrigger>(FindObjectsSortMode.None));
    }

    void Update()
    {
        CheckEnemyProximity();
    }

    public void ShowTeamSetupPanel()
    {
        teamSetupPanel.SetActive(true);
    }

    public void ShowEnemyInfo(EnemyData enemyData)
    {
        if (enemyInfoPanel != null && enemyInfoText != null)
        {
            enemyInfoPanel.SetActive(true);
            enemyInfoText.text = $"Enemy: {enemyData.Name}\nHP: {enemyData.HP}\nAttack: {enemyData.Attack}\nElement: {enemyData.Element}";
        }
    }

    public void HideEnemyInfo()
    {
        if (enemyInfoPanel != null)
        {
            enemyInfoPanel.SetActive(false);
        }
    }

    public void StartBattle(List<EnemyData> selectedEnemies, bool playerTurnFirst)
    {
        teamData.SelectedEnemies = selectedEnemies;
        isPlayerTurnFirst = playerTurnFirst;
        PlayerPrefs.SetInt("PlayerTurnFirst", playerTurnFirst ? 1 : 0);
        SceneManager.LoadScene("BattleScene");
    }

    private void CheckEnemyProximity()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(playerTransform.position, enemy.transform.position) <= enemyAttackRange)
            {
                teamData.SelectedEnemies = enemy.GetEnemyData();
                StartBattle(teamData.SelectedEnemies, false);
                break;
            }
        }
    }
}