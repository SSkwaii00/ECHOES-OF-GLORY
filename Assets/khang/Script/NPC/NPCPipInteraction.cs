using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPipInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Pip";
        questDescription = "Bắt lại 3 con gà chạy tán loạn cho Pip.";
        dialogueSequence = new List<string>
{
    "Pip: Ôi không! Mấy con gà của em chạy tứ tung, mẹ mà biết chắc mắng to!",
    "Pip: Anh/chị giúp em bắt lại 3 con quanh sân được không ạ?",
    "Người chơi: Được, để ta thử rượt mấy chú gà nghịch ấy."
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Pip: Cảm ơn anh/chị! Nhớ nhẹ tay với con Gạo, " +
                                    "đó là con gà em yêu nhất. Chúc anh/chị may mắn!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Pip: Vâng… chắc em sẽ phải tự xoay xở vậy. Cũng cảm ơn anh/chị ạ."));
        base.OnDecline();
    }
}
