using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "Data/TeamData")]
public class TeamData : ScriptableObject
{
    public List<CombatantData> SelectedCombatants = new List<CombatantData>();
    public List<EnemyData> SelectedEnemies = new List<EnemyData>();
    public List<CombatantData>[] SavedTeams = new List<CombatantData>[3];

    void OnEnable()
    {
        for (int i = 0; i < SavedTeams.Length; i++)
        {
            SavedTeams[i] = new List<CombatantData>();
        }
    }

    public bool IsTeamFull()
    {
        return SelectedCombatants.Count >= 4;
    }

    public void SetCharacter(int index, CombatantData character)
    {
        if (index >= 0 && index < SelectedCombatants.Count)
        {
            SelectedCombatants[index] = character;
        }
        else if (index >= 0 && !IsTeamFull())
        {
            SelectedCombatants.Add(character);
        }
    }

    public void ClearTeam()
    {
        SelectedCombatants.Clear();
    }
}