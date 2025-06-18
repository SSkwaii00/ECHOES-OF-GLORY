using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLendoInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Lendo";
        questDescription = "Quét lá giúp Lendo giữ sân làng sạch sẽ.";
        dialogueSequence = new List<string>
{
    "Lendo: Một ngôi làng sạch là một ngôi làng mạnh! Nhưng bọn trẻ vừa vứt lá khắp sân.",
    "Lendo: Cầm lấy chiếc chổi này và giúp tôi quét sạch 3 góc sân được chứ?",
    "Người chơi: Quét lá thôi mà, để tôi lo!"
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Lendo: Cảm ơn! Đây là ca canh gác nghiêm túc đầu tiên của tôi, " +
                                    "tôi muốn mọi thứ tươm tất để không ai phàn nàn. Bạn thật tốt bụng!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
       // StartCoroutine(TypeDialogue("Lendo: Hiểu rồi. Tôi đang định sang tiệm rèn mượn thêm chổi, " +
        //                            "có lẽ phải tự xoay xở thôi."));
        base.OnDecline();
    }
}
