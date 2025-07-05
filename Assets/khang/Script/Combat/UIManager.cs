using UnityEngine;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private UnityEngine.UI.Button[] actionButtons;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text turnCountText;

    private CombatManager combatManager;

    public void SetCombatManager(CombatManager manager)
    {
        combatManager = manager;
    }

    public void ShowActionPanel(Combatant combatant, Action<int> onActionSelected)
    {
        if (actionPanel == null || actionButtons == null || characterNameText == null || turnCountText == null)
        {
            Debug.LogError("UI components not assigned.");
            return;
        }

        actionPanel.SetActive(true);
        characterNameText.text = combatant.Name;
        CombatantData data = combatant.GetData();

        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (i < data.Skills.Length)
            {
                actionButtons[i].gameObject.SetActive(true);
                actionButtons[i].GetComponentInChildren<TMP_Text>().text = data.Skills[i].SkillName;
                int index = i;
                actionButtons[i].onClick.RemoveAllListeners();
                actionButtons[i].onClick.AddListener(() =>
                {
                    Debug.Log("Button " + index + " clicked for " + combatant.Name);
                    onActionSelected(index);
                });

                // Kích hoạt button dựa trên điều kiện
                bool isEnabled = false;
                switch (index)
                {
                    case 0: // Đánh thường
                        isEnabled = true;
                        break;
                    case 1: // Skill 2
                        isEnabled = combatant.SkillCharge >= 3;
                        break;
                    case 2: // Skill 3
                        isEnabled = combatant.Mana >= 100;
                        break;
                }
                actionButtons[i].interactable = isEnabled;
            }
            else
            {
                actionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideActionPanel()
    {
        if (actionPanel != null) actionPanel.SetActive(false);
    }

    public void UpdateTurnCount(int count)
    {
        if (turnCountText != null) turnCountText.text = "Turn: " + count;
    }
}