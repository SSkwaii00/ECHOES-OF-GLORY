using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    private int currentPlayerIndex = 0;
    private int currentEnemyIndex = 0;
    private bool isPlayerTurn = true;

    public void Initialize(List<Combatant> players, List<Enemy> enemies, bool isPlayerTurnFirst)
    {
        currentPlayerIndex = 0;
        currentEnemyIndex = 0;
        isPlayerTurn = isPlayerTurnFirst; // Thiết lập lượt đầu tiên dựa trên isPlayerTurnFirst
        DebugLogger.Log($"TurnManager initialized with isPlayerTurnFirst: {isPlayerTurnFirst}");
    }

    public (ICombatant, bool)? GetNextTurn(List<Combatant> players, List<Enemy> enemies)
    {
        var livePlayers = players.Where(p => p != null && p.HP > 0).ToList();
        var liveEnemies = enemies.Where(e => e != null && e.HP > 0).Select(e => e as ICombatant).ToList();

        if (livePlayers.Count == 0 || liveEnemies.Count == 0)
        {
            DebugLogger.Log("Battle over: No live players or enemies.");
            return null; // Kết thúc nếu một bên hết nhân vật
        }

        if (isPlayerTurn)
        {
            if (livePlayers.Count == 0)
            {
                isPlayerTurn = false;
                return GetNextTurn(players, enemies); // Chuyển sang enemy nếu không còn player
            }
            if (currentPlayerIndex >= livePlayers.Count)
            {
                currentPlayerIndex = 0; // Reset index nếu vượt quá giới hạn
            }
            var combatant = livePlayers[currentPlayerIndex] as ICombatant;
            currentPlayerIndex = (currentPlayerIndex + 1) % livePlayers.Count; // Tăng index, quay lại 0 nếu vượt quá
            combatant?.AdvanceActionValue();
            DebugLogger.Log($"Player turn: {combatant.Name}");
            isPlayerTurn = false; // Chuyển sang enemy cho lượt tiếp theo
            return (combatant, true);
        }
        else
        {
            if (liveEnemies.Count == 0)
            {
                isPlayerTurn = true;
                return GetNextTurn(players, enemies); // Chuyển sang player nếu không còn enemy
            }
            if (currentEnemyIndex >= liveEnemies.Count)
            {
                currentEnemyIndex = 0; // Reset index nếu vượt quá giới hạn
            }
            var combatant = liveEnemies[currentEnemyIndex] as ICombatant;
            currentEnemyIndex = (currentEnemyIndex + 1) % liveEnemies.Count; // Tăng index, quay lại 0 nếu vượt quá
            combatant?.AdvanceActionValue();
            DebugLogger.Log($"Enemy turn: {combatant.Name}");
            isPlayerTurn = true; // Chuyển sang player cho lượt tiếp theo
            return (combatant, false);
        }
    }

    public void TriggerPlayerTurn(List<Combatant> players, List<Enemy> enemies)
    {
        var livePlayers = players.Where(p => p != null && p.HP > 0).ToList();
        if (livePlayers.Count > 0)
        {
            isPlayerTurn = true;
            currentEnemyIndex = 0; // Reset enemy index
            DebugLogger.Log("Triggered player turn.");
        }
    }

    public void ForcePlayerTurn()
    {
        isPlayerTurn = true;
        currentEnemyIndex = 0; // Reset enemy index để đảm bảo player đi tiếp theo
        DebugLogger.Log("Forced player turn after enemy first turn.");
    }
}