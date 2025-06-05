using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    public GameObject player; // Tham chiếu đến GameObject của người chơi
    public GameObject talkButton; // Nút "Nói chuyện" trên Canvas
    public GameObject dialoguePanel; // Panel hiển thị hội thoại
    public TextMeshProUGUI dialogueText; // TextMeshProUGUI để hiển thị nội dung hội thoại
    public Button acceptButton; // Nút "Nhận"
    public Button declineButton; // Nút "Xin lỗi"
    public Button continueButton; // Nút "Tiếp tục"
    public Button skipButton; // Nút "Bỏ qua"
    public GameObject questLogPanel; // Panel bảng nhiệm vụ
    public TextMeshProUGUI questLogText; // TextMeshProUGUI để hiển thị nội dung bảng nhiệm vụ
    public float typeSpeed = 0.03f; // Tốc độ gõ chữ (giây mỗi ký tự)

    private bool isPlayerNear = false; // Kiểm tra người chơi có gần NPC không
    private bool hasTalked = false; // Kiểm tra xem đã từ chối nhiệm vụ chưa
    private bool isInDialogue = false; // Kiểm tra xem đang trong hội thoại không
    private float interactionDistance = 3f; // Khoảng cách tối đa để tương tác
    private List<string> dialogueSequence; // Danh sách các câu thoại
    private int currentDialogueIndex = 0; // Chỉ số câu thoại hiện tại
    private bool isTyping = false; // Kiểm tra xem đang gõ chữ không
    private List<string> questLog; // Danh sách nhiệm vụ

    void Start()
    {
        // Ẩn các UI khi bắt đầu
        talkButton.SetActive(false);
        dialoguePanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        questLogPanel.SetActive(false); // Ẩn bảng nhiệm vụ ban đầu

        // Gán sự kiện cho các nút
        acceptButton.onClick.AddListener(OnAccept);
        declineButton.onClick.AddListener(OnDecline);
        continueButton.onClick.AddListener(OnContinue);
        skipButton.onClick.AddListener(OnSkip);

        // Khởi tạo danh sách hội thoại
        dialogueSequence = new List<string>
        {
            "Orlin: Ngày mai là lễ truyền rèn của dòng họ tôi. Nhưng lò rèn không có đủ lửa linh. Ngươi hãy hủy diệt cuốn sách tại Hầm mộ Rỗng.",
            "Người chơi: Hầm mộ Rỗng ư? Nghe nguy hiểm đấy, nhưng tôi sẽ thử xem sao!"
        };

        // Khởi tạo bảng nhiệm vụ
        questLog = new List<string>();
        UpdateQuestLogUI();
    }

    void Update()
    {
        // Tính khoảng cách giữa người chơi và NPC
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Bật/tắt bảng nhiệm vụ bất cứ lúc nào bằng phím Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questLogPanel.SetActive(!questLogPanel.activeSelf);
        }

        // Nếu đang trong hội thoại, kiểm tra khoảng cách và phím thoát
        if (isInDialogue)
        {
            // Nếu người chơi đi xa, thoát hội thoại
            if (distance > interactionDistance)
            {
                EndDialogue();
            }

            // Nhấn phím Escape để thoát hội thoại
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();
            }
        }
        else
        {
            // Nếu người chơi ở gần và chưa từ chối nhiệm vụ
            if (distance <= interactionDistance && !hasTalked)
            {
                // Hiển thị nút "Nói chuyện"
                talkButton.SetActive(true);
                isPlayerNear = true;

                // Xoay NPC hướng về người chơi
                Vector3 direction = (player.transform.position - transform.position).normalized;
                direction.y = 0; // Giữ NPC thẳng đứng
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                talkButton.SetActive(false);
                isPlayerNear = false;
            }

            // Nhấn phím E để bắt đầu hội thoại
            if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }
    }

    void StartDialogue()
    {
        // Hiển thị panel hội thoại
        dialoguePanel.SetActive(true);
        talkButton.SetActive(false);
        isInDialogue = true; // Đánh dấu đang trong hội thoại
        currentDialogueIndex = 0;

        // Nếu chưa từ chối, bắt đầu câu thoại đầu tiên
        if (!hasTalked)
        {
            StartCoroutine(TypeDialogue(dialogueSequence[currentDialogueIndex]));
            continueButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
        }
        else
        {
            // Nếu đã từ chối, NPC buồn
            StartCoroutine(TypeDialogue("Orlin: ... (Orlin trông buồn bã và không nói gì thêm)."));
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
        }
    }

    void OnContinue()
    {
        if (isTyping)
        {
            // Nếu đang gõ, hoàn thành ngay lập tức
            StopAllCoroutines();
            dialogueText.text = dialogueSequence[currentDialogueIndex];
            isTyping = false;
            if (currentDialogueIndex == dialogueSequence.Count - 1)
            {
                acceptButton.gameObject.SetActive(true);
                declineButton.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(false);
                skipButton.gameObject.SetActive(false);
            }
        }
        else
        {
            // Chuyển sang câu thoại tiếp theo
            currentDialogueIndex++;
            if (currentDialogueIndex < dialogueSequence.Count)
            {
                StartCoroutine(TypeDialogue(dialogueSequence[currentDialogueIndex]));
                if (currentDialogueIndex == dialogueSequence.Count - 1)
                {
                    acceptButton.gameObject.SetActive(true);
                    declineButton.gameObject.SetActive(true);
                    continueButton.gameObject.SetActive(false);
                    skipButton.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnSkip()
    {
        // Dừng hiệu ứng gõ chữ và nhảy thẳng đến phần nhận/từ chối
        StopAllCoroutines();
        dialogueText.text = dialogueSequence[dialogueSequence.Count - 1];
        currentDialogueIndex = dialogueSequence.Count - 1;
        isTyping = false;
        acceptButton.gameObject.SetActive(true);
        declineButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }

    void EndDialogue()
    {
        // Dừng hiệu ứng gõ chữ
        StopAllCoroutines();

        // Đóng panel hội thoại và đặt lại trạng thái
        dialoguePanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        isInDialogue = false;
        isTyping = false;

        // Nếu người chơi vẫn ở gần và chưa từ chối, hiển thị lại nút nói chuyện
        if (isPlayerNear && !hasTalked)
        {
            talkButton.SetActive(true);
        }
    }

    void OnAccept()
    {
        // Khi chọn "Nhận"
        StartCoroutine(TypeDialogue("Orlin: Cảm ơn ngươi! Lễ truyền rèn là truyền thống lâu đời của dòng họ ta, nơi chúng ta rèn nên những thanh kiếm huyền thoại bằng lửa linh. Hãy cẩn thận ở Hầm mộ Rỗng!"));
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        // Thêm nhiệm vụ vào bảng nhiệm vụ
        string quest = "Hủy diệt cuốn sách trong Hầm mộ Rỗng để giúp Orlin tổ chức lễ truyền rèn.";
        if (!questLog.Contains(quest))
        {
            questLog.Add(quest);
            UpdateQuestLogUI();
        }
    }

    void OnDecline()
    {
        // Khi chọn "Xin lỗi"
        StartCoroutine(TypeDialogue("Orlin: ... Ta hiểu, ngươi bận. (Orlin trông buồn bã)."));
        hasTalked = true; // Đánh dấu đã từ chối, NPC sẽ không nhắc lại
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }

    void UpdateQuestLogUI()
    {
        // Cập nhật giao diện bảng nhiệm vụ
        if (questLog.Count == 0)
        {
            questLogText.text = "Không có nhiệm vụ nào.";
        }
        else
        {
            questLogText.text = "Nhiệm vụ hiện tại:\n" + string.Join("\n", questLog);
        }
    }

    IEnumerator TypeDialogue(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;

        // Hiển thị nút tiếp tục nếu chưa phải câu cuối
        if (currentDialogueIndex < dialogueSequence.Count - 1 && !hasTalked)
        {
            continueButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(true);
        }
    }

    // Hiển thị khoảng cách tương tác trong Editor để dễ debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}