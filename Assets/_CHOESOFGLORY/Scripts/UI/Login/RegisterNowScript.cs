using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RegisterNowScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("Register"); // Chuyển sang Scene Register
    }
}