using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CloseTextScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("MainMenu0"); // Chuyển về Scene MainMenu0
    }
}