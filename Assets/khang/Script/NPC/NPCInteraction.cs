using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public abstract class NPCInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject talkButton;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton;
    public Button declineButton;
    public Button continueButton;
    public Button skipButton;
    public Animator animator; // Thêm tham chiếu đến Animator

    [Header("Settings")]
    public float typeSpeed = 0.03f;
    public float interactionDistance = 5f;
    public LayerMask playerLayer;

    protected bool isPlayerNear = false;
    protected bool isInDialogue = false;
    protected bool isTyping = false;
    protected int currentDialogueIndex = 0;
    protected List<string> dialogueSequence = new List<string>();
    protected string questDescription;
    protected string npcName;

    // Biến tĩnh để theo dõi NPC gần nhất và NPC hiện tại đang nói chuyện
    private static NPCInteraction closestNPC;
    private static float closestDistance = float.MaxValue;
    private static NPCInteraction currentNPC;

    protected virtual void Start()
    {
        if (!ValidateComponents()) return;

        talkButton.SetActive(false);
        dialoguePanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        // Xóa các sự kiện cũ trước khi gán mới
        acceptButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();

        // Gán các sự kiện mới
        acceptButton.onClick.AddListener(() => OnAccept());
        declineButton.onClick.AddListener(() => OnDecline());
        continueButton.onClick.AddListener(() => OnContinue());
        skipButton.onClick.AddListener(() => OnSkip());

        InitializeDialogue();
        Debug.Log($"{npcName} khởi tạo.");

        // Khởi tạo hoạt ảnh Idle
        if (animator != null)
        {
            animator.Play("Idle"); // Chạy trạng thái Idle khi khởi tạo
        }
        else
        {
            Debug.LogWarning($"{npcName}: Animator reference is null. Please assign Animator in Inspector.");
        }
    }

    protected virtual void Update()
    {
        if (player == null)
        {
            Debug.LogError($"{npcName}: Player reference is null. Please assign Player in Inspector.");
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Chỉ cập nhật closestNPC nếu không trong hội thoại
        if (!isInDialogue && distance <= interactionDistance && CanStartDialogue())
        {
            if (closestNPC == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = this;
            }
        }
        else if (closestNPC == this && !isInDialogue)
        {
            closestNPC = null;
            closestDistance = float.MaxValue;
        }

        if (isInDialogue)
        {
            // Kiểm tra khoảng cách và thoát hội thoại
            if (distance > interactionDistance || Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();
            }
        }
        else
        {
            if (closestNPC == this)
            {
                talkButton.SetActive(true);
                isPlayerNear = true;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                talkButton.SetActive(false);
                isPlayerNear = false;
            }

            if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Phím E được nhấn, bắt đầu hội thoại với {npcName}");
                StartDialogue();
            }
        }
    }

    protected virtual void InitializeDialogue()
    {
        // Phương thức trừu tượng, được override bởi lớp con
    }

    protected virtual bool CanStartDialogue()
    {
        return true; // Có thể override để kiểm tra trạng thái nhiệm vụ
    }

    protected void StartDialogue()
    {
        // Gán NPC hiện tại và khóa hội thoại
        if (currentNPC != null && currentNPC != this)
        {
            currentNPC.EndDialogue(); // Kết thúc hội thoại của NPC khác nếu có
        }
        currentNPC = this;
        InitializeDialogue(); // Khởi tạo lại dialogueSequence cho NPC này
        dialoguePanel.SetActive(true);
        talkButton.SetActive(false);
        isInDialogue = true;
        currentDialogueIndex = 0;

        // Kích hoạt hoạt ảnh Talking
        if (animator != null)
        {
            animator.SetTrigger("StartTalking"); // Kích hoạt Trigger để chuyển sang Talking
        }
        else
        {
            Debug.LogWarning($"{npcName}: Cannot play Talking animation because Animator is null.");
        }

        // Xóa và gán lại sự kiện để đảm bảo chỉ gọi cho NPC hiện tại
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => OnAccept());

        QuestStatus status = QuestManager.Instance != null ?
            QuestManager.Instance.GetQuestStatus(questDescription) : QuestStatus.NotStarted;
        if (status == QuestStatus.NotStarted)
        {
            StartCoroutine(TypeDialogue(dialogueSequence[currentDialogueIndex]));
            continueButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
        }
        else if (status == QuestStatus.InProgress)
        {
            StartCoroutine(TypeDialogue($"{npcName}: Ngươi đã tìm được {questDescription} chưa?"));
            continueButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
        else if (status == QuestStatus.Completed)
        {
            StartCoroutine(TypeDialogue($"{npcName}: Cảm ơn ngươi đã hoàn thành nhiệm vụ!"));
            continueButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
    }

    protected void OnContinue()
    {
        if (currentNPC != this) return; // Chỉ tiếp tục nếu đây là NPC hiện tại

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueSequence[currentDialogueIndex];
            isTyping = false;
            UpdateButtons();
        }
        else
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < dialogueSequence.Count)
            {
                StartCoroutine(TypeDialogue(dialogueSequence[currentDialogueIndex]));
                UpdateButtons();
            }
        }
    }

    protected void OnSkip()
    {
        if (currentNPC != this) return; // Chỉ skip nếu đây là NPC hiện tại

        StopAllCoroutines();
        dialogueText.text = dialogueSequence[dialogueSequence.Count - 1];
        currentDialogueIndex = dialogueSequence.Count - 1;
        isTyping = false;
        UpdateButtons();
    }

    protected virtual void OnAccept()
    {
        if (currentNPC != this) return; // Chỉ chấp nhận nếu đây là NPC hiện tại

        if (!string.IsNullOrEmpty(questDescription))
        {
            Debug.Log($"Thêm nhiệm vụ từ {npcName}: {questDescription}");
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.AddQuest(questDescription, npcName);
            }
            else
            {
                Debug.LogError("QuestManager Instance is null!");
            }
        }
        else
        {
            Debug.LogError($"{npcName}: Cannot add quest. questDescription is null/empty.");
        }
        EndDialogue();
    }

    protected virtual void OnDecline()
    {
        if (currentNPC != this) return; // Chỉ từ chối nếu đây là NPC hiện tại
        EndDialogue();
    }

    protected void EndDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        isInDialogue = false;
        isTyping = false;
        if (isPlayerNear && CanStartDialogue())
        {
            talkButton.SetActive(true);
        }

        // Quay lại trạng thái Idle khi kết thúc hội thoại
        if (animator != null)
        {
            animator.Play("Idle"); // Chạy lại trạng thái Idle
        }

        // Chỉ reset currentNPC nếu đây là NPC hiện tại
        if (currentNPC == this)
        {
            currentNPC = null;
        }
        // Reset closestNPC để cho phép chọn NPC khác
        closestNPC = null;
        closestDistance = float.MaxValue;
    }

    protected IEnumerator TypeDialogue(string text)
    {
        if (currentNPC != this) yield break; // Dừng nếu không phải NPC hiện tại

        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in text)
        {
            if (!isInDialogue || currentNPC != this) yield break; // Dừng nếu hội thoại bị gián đoạn
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        UpdateButtons();
    }

    protected void UpdateButtons()
    {
        if (currentNPC != this) return; // Chỉ cập nhật nút nếu đây là NPC hiện tại

        if (currentDialogueIndex == dialogueSequence.Count - 1)
        {
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
    }

    protected bool ValidateComponents()
    {
        if (player == null || talkButton == null || dialoguePanel == null || dialogueText == null ||
            acceptButton == null || declineButton == null || continueButton == null || skipButton == null ||
            animator == null) // Kiểm tra thêm Animator
        {
            Debug.LogError($"Thiếu thành phần cần thiết trên {gameObject.name}. Vui lòng kiểm tra Inspector.");
            return false;
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}