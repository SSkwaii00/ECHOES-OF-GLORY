using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TeamSelection : MonoBehaviour
{
    public static int currentSlotIndex = -1;
    public static List<int> selectedCharacterIndices = new List<int>(); // Lưu index của character thay vì GameObject
    public Button[] slots; // Mảng để kéo thả 4 slot button từ Hierarchy
    public Button startBattleButton;
    public Button quickTeamButton; // Nút Quick Team
    public Button clearSlotButton; // Nút Clear Slot
    public List<CombatantData> availableCombatantData;
    public GameObject characterButtonPrefab;
    public Transform characterDisplayArea;
    public List<GameObject> characterPrefabs; // Danh sách Prefab nhân vật
    private Dictionary<int, int> slotCharacterIndices = new Dictionary<int, int>(); // Lưu index nhân vật cho từng slot
    private const int MAX_TEAM_SIZE = 4;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadSelectedCharacters();
    }

    void Start()
    {
        PopulateSlots();
        startBattleButton.onClick.AddListener(OnStartBattle);
        quickTeamButton.onClick.AddListener(OnQuickTeam);
        clearSlotButton.onClick.AddListener(OnClearSlot);
        startBattleButton.interactable = false;
        if (PlayerPrefs.HasKey("CurrentSlotIndex") && PlayerPrefs.HasKey("SelectedCharacterIndex"))
        {
            currentSlotIndex = PlayerPrefs.GetInt("CurrentSlotIndex");
            int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex");
            if (currentSlotIndex >= 0 && currentSlotIndex < MAX_TEAM_SIZE && selectedCharacterIndex >= 0 && selectedCharacterIndex < availableCombatantData.Count && !IsCharacterSelected(selectedCharacterIndex))
            {
                AddCharacterToSlot(currentSlotIndex, selectedCharacterIndex);
            }
            else if (IsCharacterSelected(selectedCharacterIndex))
            {
                Debug.LogWarning($"Character index {selectedCharacterIndex} is already selected in another slot.");
            }
            PlayerPrefs.DeleteKey("CurrentSlotIndex");
            PlayerPrefs.DeleteKey("SelectedCharacterIndex");
        }
        UpdateStartBattleButton();
    }

    void PopulateSlots()
    {
        if (slots == null || slots.Length < MAX_TEAM_SIZE)
        {
            Debug.LogError($"Slots array has {slots?.Length ?? 0} elements, expected {MAX_TEAM_SIZE}. Assign all slots in Inspector.");
            return;
        }

        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            int index = i;
            Button slotButton = slots[i];
            if (slotButton != null)
            {
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(() => OpenCharacterSelection(index));
                Transform slotTransform = slotButton.transform;
                TMP_Text addText = slotTransform.Find("AddText")?.GetComponent<TMP_Text>();
                Transform model = slotTransform.Find("Model");
                TMP_Text levelNameText = slotTransform.Find("LevelNameText")?.GetComponent<TMP_Text>();
                if (addText != null && model != null && levelNameText != null)
                {
                    if (!slotCharacterIndices.ContainsKey(index))
                    {
                        addText.gameObject.SetActive(true);
                        model.gameObject.SetActive(false);
                        levelNameText.gameObject.SetActive(false);
                        slotButton.interactable = true;
                    }
                    else
                    {
                        addText.gameObject.SetActive(false);
                        model.gameObject.SetActive(true);
                        levelNameText.gameObject.SetActive(true);
                        UpdateSlotModel(index, slotCharacterIndices[index]);
                        UpdateLevelNameText(index, slotCharacterIndices[index]);
                        //Debug.Log($"Slot {index} already contains character index {slotCharacterIndices[index]}");
                        slotButton.interactable = true; // Giữ button sáng để chọn lại
                    }
                }
                else
                {
                    Debug.LogWarning($"Slot {index} missing AddText, Model, or LevelNameText. Ensure all children exist.");
                }
            }
            else
            {
                Debug.LogError($"Slot {index} is not assigned in the slots array. Assign a Button in Inspector.");
            }
        }
    }

    void OpenCharacterSelection(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_TEAM_SIZE)
        {
            currentSlotIndex = slotIndex;
            PlayerPrefs.SetInt("CurrentSlotIndex", slotIndex);
            SceneManager.LoadScene("CharacterSelectionScene");
            Debug.Log($"Opening CharacterSelectionScene for slot {slotIndex}");
        }
    }

    void AddCharacterToSlot(int slotIndex, int characterIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_TEAM_SIZE && characterIndex >= 0 && characterIndex < availableCombatantData.Count && !IsCharacterSelected(characterIndex))
        {
            slotCharacterIndices[slotIndex] = characterIndex;
            selectedCharacterIndices.Add(characterIndex); // Lưu index
            PlayerPrefs.SetInt($"Slot{slotIndex}Character", characterIndex);
            UpdateSlotUI(slotIndex, null, characterIndex); // Truyền null cho combatant vì sẽ tạo lại
            //Debug.Log($"Added character index {characterIndex} to slot {slotIndex}");
            PopulateSlots(); // Cập nhật giao diện
        }
        else
        {
            Debug.LogWarning($"Character index {characterIndex} is already selected or invalid for slot {slotIndex}");
        }
    }

    void UpdateSlotUI(int slotIndex, Combatant combatant, int characterIndex)
    {
        Transform slotTransform = slots[slotIndex].transform;
        TMP_Text addText = slotTransform.Find("AddText")?.GetComponent<TMP_Text>();
        Transform model = slotTransform.Find("Model");
        TMP_Text levelNameText = slotTransform.Find("LevelNameText")?.GetComponent<TMP_Text>();
        if (addText != null && model != null && levelNameText != null)
        {
            addText.gameObject.SetActive(false);
            model.gameObject.SetActive(true);
            levelNameText.gameObject.SetActive(true);
            foreach (Transform child in model)
            {
                Destroy(child.gameObject);
            }
            if (characterIndex < characterPrefabs.Count && characterPrefabs[characterIndex] != null)
            {
                GameObject characterModel = Instantiate(characterPrefabs[characterIndex], model);
                characterModel.transform.localPosition = Vector3.zero;
                Combatant modelCombatant = characterModel.GetComponent<Combatant>();
                if (modelCombatant != null)
                {
                    if (combatant != null)
                    {
                        modelCombatant.SetData(combatant.GetData());
                    }
                    else
                    {
                        modelCombatant.SetData(availableCombatantData[characterIndex]); // Sử dụng dữ liệu từ availableCombatantData
                    }
                }
                Button modelButton = characterModel.AddComponent<Button>();
                modelButton.onClick.AddListener(() => OpenCharacterSelection(slotIndex));
            }
            else
            {
                Debug.LogWarning($"No prefab found for character index {characterIndex} in slot {slotIndex}");
            }
            UpdateLevelNameText(slotIndex, characterIndex);
        }
        UpdateStartBattleButton();
    }

    void UpdateSlotModel(int slotIndex, int characterIndex)
    {
        Transform slotTransform = slots[slotIndex].transform;
        Transform model = slotTransform.Find("Model");
        if (model != null)
        {
            foreach (Transform child in model)
            {
                Destroy(child.gameObject);
            }
            if (characterIndex < characterPrefabs.Count && characterPrefabs[characterIndex] != null)
            {
                GameObject characterModel = Instantiate(characterPrefabs[characterIndex], model);
                characterModel.transform.localPosition = Vector3.zero;
                Combatant modelCombatant = characterModel.GetComponent<Combatant>();
                if (modelCombatant != null)
                {
                    modelCombatant.SetData(availableCombatantData[characterIndex]);
                }
                Button modelButton = characterModel.AddComponent<Button>();
                modelButton.onClick.AddListener(() => OpenCharacterSelection(slotIndex));
            }
        }
    }

    void UpdateLevelNameText(int slotIndex, int characterIndex)
    {
        Transform slotTransform = slots[slotIndex].transform;
        TMP_Text levelNameText = slotTransform.Find("LevelNameText")?.GetComponent<TMP_Text>();
        if (levelNameText != null && characterIndex >= 0 && characterIndex < availableCombatantData.Count)
        {
            CombatantData data = availableCombatantData[characterIndex];
            levelNameText.text = $"Lv {data.Level} {data.Name}";
        }
    }

    void LoadSelectedCharacters()
    {
        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            if (PlayerPrefs.HasKey($"Slot{i}Character"))
            {
                int characterIndex = PlayerPrefs.GetInt($"Slot{i}Character");
                if (characterIndex >= 0 && characterIndex < availableCombatantData.Count && !IsCharacterSelected(characterIndex))
                {
                    AddCharacterToSlot(i, characterIndex);
                }
            }
        }
    }

    bool IsCharacterSelected(int characterIndex)
    {
        return slotCharacterIndices.ContainsValue(characterIndex);
    }

    void ClearCharacterDisplay()
    {
        if (characterDisplayArea != null)
        {
            foreach (Transform child in characterDisplayArea)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void UpdateStartBattleButton()
    {
        startBattleButton.interactable = selectedCharacterIndices.Count >= MAX_TEAM_SIZE;
    }

    void OnStartBattle()
    {
        if (selectedCharacterIndices.Count >= MAX_TEAM_SIZE)
        {
            SceneManager.LoadScene("BattleScene");
            Debug.Log("Loading BattleScene with 4 selected character indices");
        }
    }

    void OnQuickTeam()
    {
        if (selectedCharacterIndices.Count >= MAX_TEAM_SIZE) return; // Không thêm nếu đã đủ

        List<int> availableIndices = Enumerable.Range(0, availableCombatantData.Count).ToList();
        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            if (!slotCharacterIndices.ContainsKey(i) && availableIndices.Count > 0)
            {
                int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                availableIndices.Remove(randomIndex); // Ngăn chọn trùng
                AddCharacterToSlot(i, randomIndex);
            }
        }
        Debug.Log("Quick Team assigned.");
    }

    void OnClearSlot()
    {
        selectedCharacterIndices.Clear();
        slotCharacterIndices.Clear();
        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            PlayerPrefs.DeleteKey($"Slot{i}Character");
        }
        PlayerPrefs.Save();
        PopulateSlots(); // Reset giao diện
        UpdateStartBattleButton();
        Debug.Log("All slots cleared.");
    }
}