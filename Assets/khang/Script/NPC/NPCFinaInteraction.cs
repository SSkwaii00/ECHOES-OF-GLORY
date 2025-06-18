using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFinaInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Fina";
        questDescription = "Lấy bao bột dự trữ trong kho cho Fina.";
        dialogueSequence = new List<string>
{
    "Fina: Trời ơi, lũ mọt đã cắn hết bao bột mới! Lễ làng sắp đến còn đâu bánh?",
    "Fina: Trong kho sau chợ còn một bao bột dự trữ, bạn mang giúp tôi được không?",
    "Người chơi: Bao bột hơi nặng đấy, nhưng tôi sẽ thử!"
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Fina: Bạn tốt bụng quá! Nhờ bột này tôi sẽ nướng lại chiếc bánh " +
                                    "từng đoạt giải 'Bánh Vàng' lần đầu tiên của mình. Cẩn thận kẻo đau lưng nhé!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Fina: Ồ, mình hiểu mà. Cảm ơn bạn đã lắng nghe, để mình tính cách khác."));
        base.OnDecline();
    }
}