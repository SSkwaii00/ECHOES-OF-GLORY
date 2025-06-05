using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHarnInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Harn";
        questDescription = "Nhặt 5 mảnh gỗ quanh xưởng cho Harn.";
        dialogueSequence = new List<string>
{
    "Harn: Sáng nay ghế tôi đóng gãy khiến bà cụ suýt ngã, thật xấu hổ!",
    "Harn: Bạn nhặt giúp tôi 5 mảnh gỗ rơi quanh xưởng để sửa ghế được chứ?",
    "Người chơi: Chuyện nhỏ, tôi lo ngay."
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Harn: Đa tạ bạn! Tôi mơ một ngày mở hội chợ đồ gỗ lớn nhất vùng, " +
                                    "những chiếc ghế vững chãi thế này sẽ là điểm nhấn!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Harn: Tôi hiểu. Ai cũng bận rộn cả, để tôi tự lo vậy."));
        base.OnDecline();
    }
}