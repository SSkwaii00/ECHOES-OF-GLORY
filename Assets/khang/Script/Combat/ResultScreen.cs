using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI historyText;
    [SerializeField] private Button returnButton;

    private void Awake()
    {
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(() => SceneManager.LoadScene("TeamSelection"));
        }
    }

    public void ShowResult(bool victory, List<(string characterName, string skillName, int damage)> battleHistory, int turnCount)
    {
        gameObject.SetActive(true);
        if (resultText != null)
        {
            resultText.text = victory ? "Victory!" : "Defeat!";
        }
        if (historyText != null)
        {
            historyText.text = $"Turns: {turnCount}\n";
            foreach (var entry in battleHistory)
            {
                historyText.text += $"{entry.characterName} used {entry.skillName} for {entry.damage} damage\n";
            }
        }
    }
}