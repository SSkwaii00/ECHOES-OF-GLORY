using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NowSceneAccountManagement : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("AccountManagement");
    }
}