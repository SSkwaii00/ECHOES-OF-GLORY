using UnityEngine;

public class Mint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerQuestHandler player = other.GetComponent<PlayerQuestHandler>();
            if (player != null)
            {
                player.CollectMint();
                Destroy(gameObject); // Xóa nhánh bạc hà sau khi thu thập
            }
        }
    }
}