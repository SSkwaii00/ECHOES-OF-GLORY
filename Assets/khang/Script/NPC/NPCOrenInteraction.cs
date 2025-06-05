using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCOrenInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Oren";
        questDescription = "Sắp xếp lại 4 thùng gạo – muối đúng nhãn.";
        dialogueSequence = new List<string>
{
    "Oren: Kho lộn xộn quá! Gạo lẫn muối, ẩm mốc mất thôi.",
    "Oren: Bạn giúp tôi kéo 4 thùng về đúng vị trí ghi trên nhãn được chứ?",
    "Người chơi: Thêm chút cơ bắp cũng tốt, để tôi giúp."
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Oren: Hoan hô! Tôi vẫn ám ảnh 'thảm hoạ xếp nhầm' năm xưa " +
                                    "khi cả làng ăn cơm mặn như nước biển. Lần này thì an toàn rồi!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Oren: Không sao, khi nào rảnh ghé lại giúp tôi nhé. Tôi sẽ cố gắng tự xoay xở."));
        base.OnDecline();
    }
}