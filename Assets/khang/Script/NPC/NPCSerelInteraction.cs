using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSerelInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Serel";
        questDescription = "Nhặt 4 chiếc khăn bị gió thổi bay cho bà Serel.";
        dialogueSequence = new List<string>
{
    "Serel: Gió lớn quá! Khăn của ta bay khắp sân sau, già rồi không chạy được.",
    "Serel: Con nhặt giúp ta 4 chiếc khăn trước khi chúng dính bẩn nhé?",
    "Người chơi: Dạ, để cháu đi ngay ạ."
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Serel: Cảm ơn con. Thuở trẻ ta thêu từng chiếc khăn cho ông nhà " +
                                    "trước khi ông lên đường. Chúng là kỷ vật vô giá với ta đấy."));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Serel: Không sao đâu con, ta sẽ chờ trời lặng gió vậy. Cảm ơn con đã ghé qua."));
        base.OnDecline();
    }
}
