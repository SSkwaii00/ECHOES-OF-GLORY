using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GoScenePrivacyPolicy : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("PrivacyPolicy");
    }
}