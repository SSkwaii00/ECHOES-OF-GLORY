using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNekoInteraction : NPCInteraction
{
    protected override void InitializeDialogue()
    {
        npcName = "Neko";
        questDescription = "Lần dấu chân tìm mèo Mun cho Neko.";
        dialogueSequence = new List<string>
{
    "Neko: Mun – con mèo to xồm của tôi – lại biến mất đâu mất rồi!",
    "Neko: Bạn giúp tôi dò dấu chân quanh làng được không? Tôi lo lắm!",
    "Người chơi: Đi săn mèo à? Nghe thú vị đấy, tôi giúp!"
};
    }
protected override void OnAccept()
    {
        StartCoroutine(TypeDialogue("Neko: Cảm ơn! Hồi nhỏ tôi cứu Mun khỏi cơn bão, " +
                                    "nó như người thân của tôi vậy. Đừng để nó đi lạc xa nhé!"));
        base.OnAccept();
    }

    protected override void OnDecline()
    {
        //StartCoroutine(TypeDialogue("Neko: Không sao, có lẽ tôi sẽ tự tìm. Dù sao cũng cảm ơn bạn."));
        base.OnDecline();
    }
}
