using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLysInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Lys";
        questDescription = "Hái 4 nhánh bạc hà quanh giếng làng để giúp Lys pha thuốc hồi phục cho trẻ em.";
        dialogueSequence = new List<string>
        {
            "Lys: Trẻ con trong làng đang bị sốt nhẹ. Ta đang cố pha thuốc hồi phục, nhưng ta cần nguyên liệu.",
            "Lys: Ngươi có thể hái giúp ta 4 nhánh bạc hà quanh giếng làng không? Chúng rất cần để chữa bệnh!",
            "Người chơi: Bạc hà quanh giếng làng ư? Nghe đơn giản, nhưng ta sẽ cẩn thận!"
        };
    }

    protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Lys: Cảm ơn ngươi! Lần đầu ta tự pha thuốc, ta đã run rẩy vì lo, " +
            "nhưng cứu được một đứa trẻ. Bạc hà này sẽ giúp bọn trẻ mau khỏe. Hãy cẩn thận quanh giếng nhé!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Lys: Ôi, ta hiểu, ngươi đang có việc riêng. (Lys trông lo lắng, nhưng cố mỉm cười)."));
        base.OnDecline();
    }
}