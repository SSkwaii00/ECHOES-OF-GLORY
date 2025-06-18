using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum QuestStatus { NotStarted, InProgress, Completed }

[System.Serializable]
public class Quest
{
    public string description;
    public string npcName;
    public QuestStatus status;

    public Quest(string description, string npcName)
    {
        this.description = description;
        this.npcName = npcName;
        this.status = QuestStatus.InProgress;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public GameObject questLogPanel;
    public TextMeshProUGUI questLogText;
    private readonly List<Quest> quests = new List<Quest>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateQuestLogUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questLogPanel.SetActive(!questLogPanel.activeSelf);
            Debug.Log("Bảng nhiệm vụ: " + (questLogPanel.activeSelf ? "Hiển thị" : "Ẩn"));
        }
    }

    public void AddQuest(string description, string npcName)
    {
        if (!string.IsNullOrEmpty(description))
        {
            if (!quests.Exists(q => q.description == description))
            {
                quests.Add(new Quest(description, npcName));
                UpdateQuestLogUI();
                ShowQuestLog();
                Debug.Log($"Đã thêm nhiệm vụ từ {npcName}: {description}, Tổng nhiệm vụ: {quests.Count}");
            }
            else
            {
                Debug.Log($"Nhiệm vụ từ {npcName} đã tồn tại: {description}");
            }
        }
        else
        {
            Debug.LogError("Quest description is null or empty.");
        }
    }

    public void CompleteQuest(string description)
    {
        Quest quest = quests.Find(q => q.description == description);
        if (quest != null)
        {
            quest.status = QuestStatus.Completed;
            UpdateQuestLogUI();
            Debug.Log($"Nhiệm vụ hoàn thành: {description}");
        }
    }

    public QuestStatus GetQuestStatus(string description)
    {
        Quest quest = quests.Find(q => q.description == description);
        return quest != null ? quest.status : QuestStatus.NotStarted;
    }

    public void ShowQuestLog()
    {
        if (questLogPanel != null)
        {
            questLogPanel.SetActive(true);
            Debug.Log("Hiển thị Quest Log.");
        }
        else
        {
            Debug.LogError("questLogPanel is null in QuestManager.");
        }
    }

    void UpdateQuestLogUI()
    {
        if (questLogText != null)
        {
            if (quests.Count == 0)
            {
                questLogText.text = "Không có nhiệm vụ nào.";
            }
            else
            {
                questLogText.text = "Nhiệm vụ hiện tại:\n";
                foreach (Quest quest in quests)
                {
                    string status = quest.status == QuestStatus.InProgress ? "[Đang thực hiện]" : "[Hoàn thành]";
                    questLogText.text += $"• {quest.description} {status}\n";
                }
            }
            Debug.Log($"Cập nhật UI bảng nhiệm vụ: {questLogText.text}");
        }
        else
        {
            Debug.LogError("questLogText is null in QuestManager.");
        }
    }
}