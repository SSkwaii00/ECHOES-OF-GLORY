using UnityEngine;
using UnityEngine.EventSystems;

public class CloseImage : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetImage; // Gán image cần ẩn vào đây trong Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.SetActive(false); // Ẩn image
        }
    }
}