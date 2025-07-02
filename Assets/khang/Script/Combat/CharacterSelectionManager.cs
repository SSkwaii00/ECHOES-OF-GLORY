using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private TeamData teamData;
    [SerializeField] private List<CombatantData> availableCharacters;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private TextMeshProUGUI[] characterNameTexts;
    [SerializeField] private Image[] characterImages;
    [SerializeField] private TextMeshProUGUI[] characterLevelTexts;
    [SerializeField] private Button confirmButton;

    private int selectedIndex = -1;

    void Start()
    {
        if (teamData == null || availableCharacters == null || characterButtons == null || characterNameTexts == null || characterImages == null || characterLevelTexts == null || confirmButton == null)
        {
            DebugLogger.LogError("CharacterSelectionManager components not assigned.");
            return;
        }

        UpdateCharacterUI();
        confirmButton.interactable = teamData.IsTeamFull();
        confirmButton.onClick.AddListener(OnConfirmTeam);
    }

    private void UpdateCharacterUI()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            if (i < availableCharacters.Count)
            {
                CombatantData character = availableCharacters[i];
                characterButtons[i].gameObject.SetActive(true);
                characterNameTexts[i].text = character.Name;
                characterImages[i].sprite = character.AvatarSprite;
                characterLevelTexts[i].text = $"Lv {character.Level}";
                characterButtons[i].onClick.RemoveAllListeners();
                characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
            }
            else
            {
                characterButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectCharacter(int index)
    {
        selectedIndex = index;
        if (selectedIndex >= 0 && selectedIndex < availableCharacters.Count)
        {
            CombatantData selectedCharacter = availableCharacters[selectedIndex];
            if (!teamData.IsTeamFull())
            {
                teamData.SetCharacter(teamData.SelectedCombatants.Count, selectedCharacter);
                DebugLogger.Log($"{selectedCharacter.Name} added to team.");
                confirmButton.interactable = teamData.IsTeamFull();
            }
        }
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
}