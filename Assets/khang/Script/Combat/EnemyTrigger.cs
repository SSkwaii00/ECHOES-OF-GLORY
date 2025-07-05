using UnityEngine;
using System.Collections.Generic;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private List<EnemyData> enemyData;
    private MapManager mapManager;

    void Start()
    {
        mapManager = FindFirstObjectByType<MapManager>();
        if (mapManager == null)
        {
            DebugLogger.LogError("MapManager not found.");
        }
    }

    public List<EnemyData> GetEnemyData()
    {
        return enemyData;
    }

    void OnMouseEnter()
    {
        if (enemyData != null && enemyData.Count > 0)
        {
            mapManager.ShowEnemyInfo(enemyData[0]);
        }
    }

    void OnMouseExit()
    {
        mapManager.HideEnemyInfo();
    }

    void OnMouseDown()
    {
        if (enemyData != null && enemyData.Count > 0)
        {
            mapManager.StartBattle(enemyData, true);
        }
    }
}