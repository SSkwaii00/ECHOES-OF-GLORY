using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NowSceneAudioSetting : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("AudioSetting");
    }
}