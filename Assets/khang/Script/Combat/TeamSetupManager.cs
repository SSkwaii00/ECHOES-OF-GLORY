using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TeamSetupManager : MonoBehaviour
{
    [SerializeField] private TeamData teamData;
    [SerializeField] private List<CombatantData> availableCharacters;
    [SerializeField] private UnityEngine.UI.Button[] teamOptionButtons; // 3 buttons for 3 teams
    [SerializeField] private UnityEngine.UI.Button[] characterSlots; // 4 slots
    [SerializeField] private TextMeshProUGUI[] slotTexts;
    [SerializeField] private UnityEngine.UI.Image[] slotImages;
    [SerializeField] private UnityEngine.UI.Button saveButton;
    [SerializeField] private UnityEngine.UI.Button quickSelectButton;
    [SerializeField] private UnityEngine.UI.Button clearButton;
    [SerializeField] private UnityEngine.UI.Button backButton; // Button to return to Map
    [SerializeField] private UnityEngine.UI.Button confirmButton; // Button to confirm character selection
    [SerializeField] private GameObject teamSetupPanel; // Reference to the parent panel
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private Transform characterModelSpawnPoint;
    [SerializeField] private TextMeshProUGUI characterInfoText;
    [SerializeField] private Transform scrollViewContent; // Use Transform for ScrollView content
    [SerializeField] private GameObject buttonAvatarPrefab;

    private List<CombatantData>[] savedTeams = new List<CombatantData>[3];
    private int currentTeamIndex = 0;
    private CombatantData selectedCharacter;
    private GameObject currentModel;
    private List<CombatantData> selectedCharactersInTeam = new List<CombatantData>(); // Track selected characters

    void Start()
    {
        if (teamData == null || availableCharacters == null || teamOptionButtons == null || characterSlots == null || slotTexts == null || slotImages == null || saveButton == null || quickSelectButton == null || clearButton == null || backButton == null || confirmButton == null || teamSetupPanel == null || characterSelectionPanel == null || characterModelSpawnPoint == null || characterInfoText == null || scrollViewContent == null || buttonAvatarPrefab == null)
        {
            DebugLogger.LogError("TeamSetupManager components not assigned.");
            return;
        }

        for (int i = 0; i < savedTeams.Length; i++)
        {
            savedTeams[i] = new List<CombatantData>();
        }

        SetupUI();
        characterSelectionPanel.SetActive(false);
        saveButton.interactable = false;
    }

    private void SetupUI()
    {
        for (int i = 0; i < teamOptionButtons.Length; i++)
        {
            int index = i;
            teamOptionButtons[i].onClick.AddListener(() => SelectTeam(index));
        }

        for (int i = 0; i < characterSlots.Length; i++)
        {
            int index = i;
            characterSlots[i].onClick.AddListener(() => OpenCharacterSelection(index));
        }

        saveButton.onClick.AddListener(SaveTeam);
        quickSelectButton.onClick.AddListener(QuickSelectTeam);
        clearButton.onClick.AddListener(ClearTeam);
        backButton.onClick.AddListener(ReturnToMap);
        confirmButton.onClick.AddListener(ConfirmCharacterSelection);

        UpdateSlotUI();
        PopulateScrollView();
    }

    private void SelectTeam(int teamIndex)
    {
        currentTeamIndex = teamIndex;
        teamData.SelectedCombatants = savedTeams[teamIndex];
        selectedCharactersInTeam.Clear();
        foreach (var character in teamData.SelectedCombatants)
        {
            if (character != null) selectedCharactersInTeam.Add(character);
        }
        UpdateSlotUI();
        saveButton.interactable = teamData.IsTeamFull();
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (i < teamData.SelectedCombatants.Count && teamData.SelectedCombatants[i] != null)
            {
                CombatantData character = teamData.SelectedCombatants[i];
                slotTexts[i].text = character.Name;
                slotImages[i].sprite = character.AvatarSprite;
                foreach (Transform child in characterSlots[i].transform)
                {
                    if (child != slotImages[i].transform && child != slotTexts[i].transform)
                        Destroy(child.gameObject);
                }
                if (character.Prefab != null)
                {
                    GameObject model = Instantiate(character.Prefab, characterSlots[i].transform);
                    model.transform.localPosition = Vector3.zero;
                    model.transform.localScale = Vector3.one * 0.5f;
                    model.transform.SetParent(characterSlots[i].transform, false);
                }
            }
            else
            {
                slotTexts[i].text = "+";
                slotImages[i].sprite = null;
                foreach (Transform child in characterSlots[i].transform)
                {
                    if (child != slotImages[i].transform && child != slotTexts[i].transform)
                        Destroy(child.gameObject);
                }
            }
        }
        saveButton.interactable = teamData.IsTeamFull();
    }

    private void OpenCharacterSelection(int slotIndex)
    {
        characterSelectionPanel.SetActive(true);
        selectedCharacter = null;
        if (currentModel != null) Destroy(currentModel);
        characterInfoText.text = "";
        PopulateScrollView();
    }

    private void PopulateScrollView()
    {
        // Xóa các button cũ
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        DebugLogger.Log($"Populating ScrollView with {availableCharacters.Count} available characters");

        // Tạo button cho từng nhân vật có sẵn
        int buttonCount = 0;
        foreach (var character in availableCharacters)
        {
            if (!selectedCharactersInTeam.Contains(character))
            {
                DebugLogger.Log($"Adding character {character.Name} to ScrollView");
                GameObject buttonObj = Instantiate(buttonAvatarPrefab, scrollViewContent, false); // false để giữ RectTransform
                UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();
                UnityEngine.UI.Image avatar = buttonObj.GetComponentInChildren<UnityEngine.UI.Image>();
                TextMeshProUGUI levelText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                // Đặt lại RectTransform để đảm bảo button nằm trong ScrollView
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero; // Đặt vị trí cục bộ về 0
                rectTransform.localScale = Vector3.one; // Đảm bảo scale đúng
                rectTransform.anchorMin = Vector2.zero; // Căn trái dưới
                rectTransform.anchorMax = Vector2.zero; // Căn trái dưới
                rectTransform.pivot = new Vector2(0.5f, 0.5f); // Cân bằng pivot

                avatar.sprite = character.AvatarSprite;
                levelText.text = $"Lv {character.Level}";
                button.onClick.AddListener(() => SelectCharacter(character));
                buttonCount++;
            }
        }

        DebugLogger.Log($"Added {buttonCount} buttons to ScrollView");

        // Buộc cập nhật layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    private void SelectCharacter(CombatantData character)
    {
        selectedCharacter = character;
        characterInfoText.text = $"Name: {character.Name}\nHP: {character.HP}\nAttack: {character.Attack}\nElement: {character.Element}\nPath: {character.Path}";

        if (currentModel != null) Destroy(currentModel);
        if (character.Prefab != null)
        {
            currentModel = Instantiate(character.Prefab, characterModelSpawnPoint);
            currentModel.transform.localPosition = Vector3.zero;
            currentModel.transform.localScale = Vector3.one;
            currentModel.transform.SetParent(characterModelSpawnPoint, false);
        }
    }

    private void ConfirmCharacterSelection()
    {
        if (selectedCharacter != null && !teamData.IsTeamFull() && !selectedCharactersInTeam.Contains(selectedCharacter))
        {
            int slotIndex = teamData.SelectedCombatants.Count;
            teamData.SetCharacter(slotIndex, selectedCharacter);
            selectedCharactersInTeam.Add(selectedCharacter);
            UpdateSlotUI();
            characterSelectionPanel.SetActive(false);
            if (currentModel != null) Destroy(currentModel);
        }
    }

    private void SaveTeam()
    {
        if (teamData.IsTeamFull())
        {
            savedTeams[currentTeamIndex] = new List<CombatantData>(teamData.SelectedCombatants);
            DebugLogger.Log($"Team {currentTeamIndex + 1} saved.");
            teamSetupPanel.SetActive(false);
        }
    }

    private void QuickSelectTeam()
    {
        teamData.ClearTeam();
        selectedCharactersInTeam.Clear();
        List<CombatantData> randomCharacters = new List<CombatantData>(availableCharacters);
        randomCharacters.Sort((a, b) => Random.value > 0.5f ? 1 : -1);
        for (int i = 0; i < 4 && i < randomCharacters.Count; i++)
        {
            if (!selectedCharactersInTeam.Contains(randomCharacters[i]))
            {
                teamData.SetCharacter(i, randomCharacters[i]);
                selectedCharactersInTeam.Add(randomCharacters[i]);
            }
        }
        UpdateSlotUI();
    }

    private void ClearTeam()
    {
        teamData.ClearTeam();
        selectedCharactersInTeam.Clear();
        UpdateSlotUI();
    }

    private void ReturnToMap()
    {
        SceneManager.LoadScene("Map");
    }
}