using UnityEngine;
using TMPro;

public class CharacterUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI energyText;

    private ICombatant combatant;

    public void SetCombatant(ICombatant c)
    {
        combatant = c;
        UpdateUI();
    }

    void Update()
    {
        if (combatant != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (hpText != null) hpText.text = $"{combatant.Name} HP: {combatant.HP}";
        if (manaText != null) manaText.text = $"MP: {combatant.Mana}";
        if (energyText != null) energyText.text = $"Energy: {combatant.Energy}";
    }
}