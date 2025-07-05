using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TeamSelection : MonoBehaviour
{
    [SerializeField] private TeamData teamData;
    [SerializeField] private List<CombatantData> availableCharacters;
    [SerializeField] private List<EnemyData> availableEnemies;
    [SerializeField] private Button[] characterSlots;
    [SerializeField] private Button[] enemySlots;
    [SerializeField] private TextMeshProUGUI[] characterSlotTexts;
    [SerializeField] private Image[] characterSlotImages;
    [SerializeField] private TextMeshProUGUI[] enemySlotTexts;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button clearButton;

    void Start()
    {
        if (teamData == null || availableCharacters == null || availableEnemies == null || characterSlots == null || enemySlots == null || characterSlotTexts == null || characterSlotImages == null || enemySlotTexts == null || confirmButton == null || clearButton == null)
        {
            DebugLogger.LogError("TeamSelection components not assigned.");
            return;
        }

        UpdateTeamUI();
        SetupButtons();
    }

    private void UpdateTeamUI()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (i < teamData.SelectedCombatants.Count)
            {
                CombatantData character = teamData.SelectedCombatants[i];
                characterSlotTexts[i].text = $"{character.Name} (Lv {character.Level})";
                characterSlotImages[i].sprite = character.AvatarSprite;
            }
            else
            {
                characterSlotTexts[i].text = "Empty";
                characterSlotImages[i].sprite = null;
            }
        }

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (i < teamData.SelectedEnemies.Count && teamData.SelectedEnemies[i] != null)
            {
                enemySlotTexts[i].text = teamData.SelectedEnemies[i].Name;
            }
            else
            {
                enemySlotTexts[i].text = "Empty";
            }
        }

        confirmButton.interactable = teamData.IsTeamFull();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            int index = i;
            characterSlots[i].onClick.AddListener(() => OpenCharacterSelection(index));
        }

        for (int i = 0; i < enemySlots.Length; i++)
        {
            int index = i;
            enemySlots[i].onClick.AddListener(() => SelectEnemy(index));
        }

        confirmButton.onClick.AddListener(OnConfirmTeam);
        clearButton.onClick.AddListener(OnClearTeam);
    }

    private void OpenCharacterSelection(int slotIndex)
    {
        if (!teamData.IsTeamFull())
        {
            // Giả sử mở một panel chọn nhân vật
            SelectCharacterForSlot(slotIndex, availableCharacters[Random.Range(0, availableCharacters.Count)]);
        }
    }

    private void SelectCharacterForSlot(int slotIndex, CombatantData character)
    {
        teamData.SetCharacter(slotIndex, character);
        UpdateTeamUI();
    }

    private void SelectEnemy(int slotIndex)
    {
        if (slotIndex < teamData.SelectedEnemies.Count)
        {
            teamData.SelectedEnemies[slotIndex] = availableEnemies[Random.Range(0, availableEnemies.Count)];
        }
        else
        {
            teamData.SelectedEnemies.Add(availableEnemies[Random.Range(0, availableEnemies.Count)]);
        }
        UpdateTeamUI();
    }

    private void OnConfirmTeam()
    {
        if (teamData.IsTeamFull())
        {
            SceneManager.LoadScene("BattleScene");
        }
        else
        {
            DebugLogger.LogWarning("Team is not full. Please select more characters.");
        }
    }

    private void OnClearTeam()
    {
        teamData.ClearTeam();
        UpdateTeamUI();
    }
}