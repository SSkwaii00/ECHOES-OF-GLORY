using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NowSceneGlobalSetting : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("GlobalSetting");
    }
}