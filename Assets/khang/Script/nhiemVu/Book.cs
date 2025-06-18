using UnityEngine;

public class Book : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerQuestHandler player = other.GetComponent<PlayerQuestHandler>();
            if (player != null)
            {
                player.DestroyBook();
                Destroy(gameObject);
            }
        }
    }
}