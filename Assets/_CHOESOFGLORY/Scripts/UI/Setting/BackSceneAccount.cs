using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BackSceneAccount : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("AccountManagement");
    }
}