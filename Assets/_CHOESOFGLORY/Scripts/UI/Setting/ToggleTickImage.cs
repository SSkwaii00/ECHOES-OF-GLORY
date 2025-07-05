using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleTickImage : MonoBehaviour, IPointerClickHandler
{
    public GameObject tickImage; // Gán image tick đã ẩn vào đây trong Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tickImage != null)
        {
            tickImage.SetActive(!tickImage.activeSelf); // Toggle trạng thái hiển thị/ẩn của tick
        }
    }
}