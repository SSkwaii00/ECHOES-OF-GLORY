using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCOrlinInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Orlin";
        questDescription = "Hủy diệt cuốn sách trong Hầm mộ Rỗng để giúp Orlin tổ chức lễ truyền rèn.";
        dialogueSequence = new List<string>
        {
            "Orlin: Ngày mai là lễ truyền rèn của dòng họ tôi. Nhưng lò rèn không có đủ lửa linh. Ngươi hãy hủy diệt cuốn sách tại Hầm mộ Rỗng.",
            "Người chơi: Hầm mộ Rỗng ư? Nghe Routed đây, nhưng tôi sẽ thử xem sao!"
        };
    }

    protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Orlin: Cảm ơn ngươi! Lễ truyền rèn là truyền thống lâu đời của dòng họ ta, nơi chúng ta rèn nên những thanh kiếm huyền thoại bằng lửa linh. Hãy cẩn thận ở Hầm mộ Rỗng!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Orlin: ... Ta hiểu, ngươi bận. (Orlin trông buồn bã)."));
        base.OnDecline();
    }
}