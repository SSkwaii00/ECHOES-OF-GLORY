using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public Button confirmButton;
    public GameObject detailPanel;
    public Transform characterDisplayArea;
    public GameObject characterButtonPrefab;
    public List<CombatantData> availableCombatantData;
    public List<GameObject> characterPrefabs; // Danh sách Prefab nhân vật
    public Transform content; // Content của ScrollView
    private int selectedCharacterIndex = -1;
    private int previousSelectedIndex = -1; // Lưu chỉ số button trước đó trong slot hiện tại
    private Dictionary<int, Button> characterButtons = new Dictionary<int, Button>(); // Lưu button
    private Dictionary<int, Image> characterImages = new Dictionary<int, Image>(); // Lưu avatar
    private Dictionary<int, TMP_Text> characterLevelTexts = new Dictionary<int, TMP_Text>(); // Lưu text Lv

    void Start()
    {
        if (content == null || characterButtonPrefab == null || availableCombatantData == null || characterPrefabs == null)
        {
            Debug.LogError("Missing required components in CharacterSelectionManager.");
            return;
        }

        PopulateCharacterList();
        confirmButton.onClick.AddListener(OnConfirm);
        confirmButton.interactable = false;
        detailPanel.SetActive(false);

        // Khôi phục trạng thái khi quay lại từ TeamSelectionScene
        int currentSlot = PlayerPrefs.GetInt("CurrentSlotIndex", -1);
        if (currentSlot >= 0 && PlayerPrefs.HasKey($"Slot{currentSlot}Character"))
        {
            int preselectedIndex = PlayerPrefs.GetInt($"Slot{currentSlot}Character");
            if (preselectedIndex >= 0 && preselectedIndex < availableCombatantData.Count)
            {
                previousSelectedIndex = preselectedIndex; // Cập nhật chỉ số trước đó
                OnCharacterSelected(preselectedIndex); // Hiển thị Prefab và thông tin
                if (characterButtons.ContainsKey(preselectedIndex))
                {
                    characterButtons[preselectedIndex].interactable = false; // Giữ xám trong slot hiện tại
                    GrayOutCharacter(preselectedIndex, true);
                }
            }
        }

        // Kiểm tra và xám các nhân vật đã gán ở slot khác
        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            if (i != currentSlot && PlayerPrefs.HasKey($"Slot{i}Character"))
            {
                int occupiedIndex = PlayerPrefs.GetInt($"Slot{i}Character");
                if (characterButtons.ContainsKey(occupiedIndex))
                {
                    characterButtons[occupiedIndex].interactable = false;
                    GrayOutCharacter(occupiedIndex, true);
                }
            }
        }
    }

    void PopulateCharacterList()
    {
        for (int i = 0; i < availableCombatantData.Count; i++)
        {
            int index = i;
            GameObject buttonObj = Instantiate(characterButtonPrefab, content);
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnCharacterSelected(index));
                characterButtons[index] = button;

                Image avatarImage = buttonObj.transform.Find("AvatarImage")?.GetComponent<Image>();
                TMP_Text levelText = buttonObj.transform.Find("LevelText")?.GetComponent<TMP_Text>();
                if (avatarImage != null && levelText != null)
                {
                    characterImages[index] = avatarImage;
                    characterLevelTexts[index] = levelText;
                    if (availableCombatantData[index].AvatarSprite != null)
                    {
                        avatarImage.sprite = availableCombatantData[index].AvatarSprite;
                    }
                    levelText.text = $"Lv {availableCombatantData[index].Level}";

                    // Kiểm tra nếu nhân vật đã gán ở slot khác
                    for (int slot = 0; slot < MAX_TEAM_SIZE; slot++)
                    {
                        if (PlayerPrefs.HasKey($"Slot{slot}Character"))
                        {
                            int occupiedIndex = PlayerPrefs.GetInt($"Slot{slot}Character");
                            if (occupiedIndex == index)
                            {
                                button.interactable = false;
                                GrayOutCharacter(index, true);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    void OnCharacterSelected(int index)
    {
        selectedCharacterIndex = index;
        confirmButton.interactable = true;
        detailPanel.SetActive(true);

        TMP_Text nameText = detailPanel.transform.Find("NameText")?.GetComponent<TMP_Text>();
        TMP_Text levelText = detailPanel.transform.Find("LevelText")?.GetComponent<TMP_Text>();
        TMP_Text hpText = detailPanel.transform.Find("HPText")?.GetComponent<TMP_Text>();
        TMP_Text atkText = detailPanel.transform.Find("ATKText")?.GetComponent<TMP_Text>();
        TMP_Text defText = detailPanel.transform.Find("DEFText")?.GetComponent<TMP_Text>();
        TMP_Text spdText = detailPanel.transform.Find("SPDText")?.GetComponent<TMP_Text>();
        TMP_Text critRateText = detailPanel.transform.Find("CRITRateText")?.GetComponent<TMP_Text>();
        TMP_Text critDMGText = detailPanel.transform.Find("CRITDMGText")?.GetComponent<TMP_Text>();

        CombatantData data = availableCombatantData[index];
        if (nameText != null) nameText.text = data.Name;
        if (levelText != null) levelText.text = $"Level: {data.Level}";
        if (hpText != null) hpText.text = $"HP: {data.HP}";
        if (atkText != null) atkText.text = $"ATK: {data.Attack}";
        if (defText != null) defText.text = $"DEF: {data.Defense}";
        if (spdText != null) spdText.text = $"SPD: {data.Agility}";
        if (critRateText != null) critRateText.text = $"CRIT Rate: {data.CritRate}%";
        if (critDMGText != null) critDMGText.text = $"CRIT DMG: {data.CritDamage}%";

        ClearCharacterDisplay();
        if (index < characterPrefabs.Count && characterPrefabs[index] != null)
        {
            GameObject characterModel = Instantiate(characterPrefabs[index], characterDisplayArea);
            characterModel.transform.localPosition = Vector3.zero;
            Combatant combatant = characterModel.GetComponent<Combatant>();
            if (combatant != null)
            {
                combatant.SetData(data);
            }
        }

        // Khôi phục màu cho button trước đó trong slot hiện tại nếu không bị gán ở slot khác
        if (previousSelectedIndex >= 0 && characterButtons.ContainsKey(previousSelectedIndex) && !IsCharacterSelectedInAnySlot(previousSelectedIndex))
        {
            characterButtons[previousSelectedIndex].interactable = true;
            GrayOutCharacter(previousSelectedIndex, false);
        }

        // Xám button mới chọn
        if (characterButtons.ContainsKey(index))
        {
            characterButtons[index].interactable = false;
            GrayOutCharacter(index, true);
            previousSelectedIndex = index; // Cập nhật chỉ số trước đó
        }

        // Kiểm tra và giữ xám các button đã gán ở slot khác
        foreach (var btn in characterButtons)
        {
            if (btn.Key != index && IsCharacterSelectedInAnySlot(btn.Key))
            {
                btn.Value.interactable = false;
                GrayOutCharacter(btn.Key, true);
            }
        }
    }

    void GrayOutCharacter(int index, bool gray)
    {
        if (characterImages.ContainsKey(index) && characterLevelTexts.ContainsKey(index))
        {
            Image image = characterImages[index];
            TMP_Text levelText = characterLevelTexts[index];
            if (gray)
            {
                image.color = new Color(0.5f, 0.5f, 0.5f); // Tối lại (grayscale)
                levelText.color = new Color(0.5f, 0.5f, 0.5f); // Tối text
            }
            else
            {
                image.color = Color.white; // Khôi phục
                levelText.color = Color.white; // Khôi phục text
            }
        }
    }

    bool IsCharacterSelectedInAnySlot(int characterIndex)
    {
        for (int i = 0; i < MAX_TEAM_SIZE; i++)
        {
            if (PlayerPrefs.HasKey($"Slot{i}Character"))
            {
                int slotIndex = PlayerPrefs.GetInt($"Slot{i}Character");
                if (slotIndex == characterIndex) return true;
            }
        }
        return false;
    }

    void ClearCharacterDisplay()
    {
        foreach (Transform child in characterDisplayArea)
        {
            Destroy(child.gameObject);
        }
    }

    void OnConfirm()
    {
        if (selectedCharacterIndex >= 0 && TeamSelection.currentSlotIndex >= 0)
        {
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
            PlayerPrefs.SetInt("CurrentSlotIndex", TeamSelection.currentSlotIndex);
            SceneManager.LoadScene("TeamSelectionScene");
            Debug.Log($"Confirmed selection: Slot {TeamSelection.currentSlotIndex} with character index {selectedCharacterIndex}");
        }
    }

    private const int MAX_TEAM_SIZE = 4; // Định nghĩa hằng số
}