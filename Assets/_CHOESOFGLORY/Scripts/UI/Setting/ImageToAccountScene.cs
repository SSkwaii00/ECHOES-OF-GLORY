using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ImageToAccountScene : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("AccountManagement");
    }
}