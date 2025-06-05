using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCYunaInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Yuna";
        questDescription = "Tìm 3 loài hoa cho Yuna kết vòng hoa dự lễ hội.";
        dialogueSequence = new List<string>
        {
            "Yuna: Lễ hội sắp tới cần vòng hoa từ 3 loại hoa khác nhau. Giúp ta tìm Hoa Sao, Hoa Mộng và Cúc Trắng quanh làng nhé.",
            "Người chơi: Ba loài hoa à? Được, mình sẽ thử tìm xem."
        };
    }

protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Yuna: Tuyệt quá! Chị gái mình từng dạy kết vòng hoa, nhưng chị đã rời làng. Cảm ơn bạn nhiều, chúc may mắn nhé!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Yuna: Ồ… không sao đâu, mình sẽ nhờ người khác vậy. " ));
        base.OnDecline();
    }
}