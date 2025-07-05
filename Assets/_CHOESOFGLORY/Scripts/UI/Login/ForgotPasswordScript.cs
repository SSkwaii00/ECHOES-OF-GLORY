using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ForgotPasswordScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("ForgotPassword"); // Chuyển sang Scene ForgotPassword
    }
}